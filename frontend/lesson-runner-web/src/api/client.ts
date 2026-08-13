/**
 * Bazowy adres API:
 * - jawna wartość `VITE_API_BASE_URL` zawsze wygrywa (także pusta - oznacza ścieżki relatywne),
 * - w trybie developerskim domyślnie backend z `dotnet run` na porcie 5000,
 * - w buildzie produkcyjnym bez konfiguracji zostają ścieżki relatywne, bo obraz web serwuje
 *   aplikację przez nginx, który proxuje `/api`, `/uploads` i `/download` do backendu.
 *   Dzięki temu w obrazie nie ląduje zaszyty `http://localhost:5000`.
 */
const configuredBaseUrl = import.meta.env.VITE_API_BASE_URL;
const apiBaseUrl = (configuredBaseUrl ?? (import.meta.env.DEV ? "http://localhost:5000" : "")).replace(/\/+$/, "");

export class ApiError extends Error {
  readonly status: number;

  constructor(status: number, message: string) {
    super(message);
    this.name = "ApiError";
    this.status = status;
  }
}

let authToken: string | null = null;
let onUnauthorized: (() => void) | null = null;

export function setAuthToken(token: string | null) {
  authToken = token;
}

export function setUnauthorizedHandler(handler: (() => void) | null) {
  onUnauthorized = handler;
}

type RequestOptions = {
  method?: string;
  body?: unknown;
};

async function request<TResponse>(path: string, options: RequestOptions = {}): Promise<TResponse> {
  const headers: Record<string, string> = {};

  if (options.body !== undefined) {
    headers["Content-Type"] = "application/json";
  }

  if (authToken) {
    headers["Authorization"] = `Bearer ${authToken}`;
  }

  const response = await fetch(`${apiBaseUrl}${path}`, {
    method: options.method ?? "GET",
    headers,
    body: options.body === undefined ? undefined : JSON.stringify(options.body),
  });

  if (response.status === 401) {
    onUnauthorized?.();
    throw new ApiError(401, "Sesja wygasła lub brak dostępu. Zaloguj się ponownie.");
  }

  if (!response.ok) {
    let message = `API request failed: ${response.status}`;

    try {
      const data = (await response.json()) as { error?: string };
      if (data?.error) {
        message = data.error;
      }
    } catch {
      // brak treści błędu - zostaje komunikat domyślny
    }

    throw new ApiError(response.status, message);
  }

  if (response.status === 204) {
    return undefined as TResponse;
  }

  return response.json() as Promise<TResponse>;
}

export function apiGet<TResponse>(path: string): Promise<TResponse> {
  return request<TResponse>(path);
}

export function apiPost<TRequest, TResponse>(path: string, body: TRequest): Promise<TResponse> {
  return request<TResponse>(path, { method: "POST", body });
}

export function apiPostEmpty<TResponse>(path: string): Promise<TResponse> {
  return request<TResponse>(path, { method: "POST" });
}

export function apiPut<TRequest, TResponse>(path: string, body: TRequest): Promise<TResponse> {
  return request<TResponse>(path, { method: "PUT", body });
}

export function apiPutEmpty<TResponse>(path: string): Promise<TResponse> {
  return request<TResponse>(path, { method: "PUT" });
}

export function apiDelete<TResponse = void>(path: string): Promise<TResponse> {
  return request<TResponse>(path, { method: "DELETE" });
}

export type DownloadedFile = {
  blob: Blob;
  fileName: string;
};

export async function apiDownload(path: string): Promise<DownloadedFile> {
  const headers: Record<string, string> = {};

  if (authToken) {
    headers["Authorization"] = `Bearer ${authToken}`;
  }

  const response = await fetch(`${apiBaseUrl}${path}`, { headers });

  if (response.status === 401) {
    onUnauthorized?.();
    throw new ApiError(401, "Sesja wygasła lub brak dostępu. Zaloguj się ponownie.");
  }

  if (!response.ok) {
    let message = `API request failed: ${response.status}`;

    try {
      const data = (await response.json()) as { error?: string };
      if (data?.error) {
        message = data.error;
      }
    } catch {
      // brak treści błędu - zostaje komunikat domyślny
    }

    throw new ApiError(response.status, message);
  }

  return {
    blob: await response.blob(),
    fileName: fileNameFromDisposition(response.headers.get("content-disposition")) ?? "export.csv",
  };
}

function fileNameFromDisposition(disposition: string | null): string | null {
  if (!disposition) {
    return null;
  }

  const utfMatch = /filename\*=UTF-8''([^;]+)/i.exec(disposition);
  if (utfMatch?.[1]) {
    return decodeURIComponent(utfMatch[1]);
  }

  const asciiMatch = /filename=\"?([^\";]+)\"?/i.exec(disposition);
  return asciiMatch?.[1] ?? null;
}

/**
 * Przesyła pojedynczy plik jako multipart/form-data.
 * Nie ustawiamy ręcznie Content-Type - przeglądarka doda boundary.
 * Przy błędzie staramy się pokazać komunikat z backendu (`{ error }`).
 */
export async function uploadFile<TResponse>(file: File): Promise<TResponse> {
  const formData = new FormData();
  formData.append("file", file);

  const headers: Record<string, string> = {};

  if (authToken) {
    headers["Authorization"] = `Bearer ${authToken}`;
  }

  const response = await fetch(`${apiBaseUrl}/api/files`, {
    method: "POST",
    headers,
    body: formData,
  });

  if (response.status === 401) {
    onUnauthorized?.();
    throw new ApiError(401, "Sesja wygasła lub brak dostępu. Zaloguj się ponownie.");
  }

  if (!response.ok) {
    let message = `Nie udało się przesłać pliku (${response.status}).`;

    try {
      const data = (await response.json()) as { error?: string };
      if (data?.error) {
        message = data.error;
      }
    } catch {
      // brak treści błędu - zostaje komunikat domyślny
    }

    throw new ApiError(response.status, message);
  }

  return response.json() as Promise<TResponse>;
}

/** Buduje pełny adres zasobu serwowanego przez backend (np. /uploads/...). */
export function resolveAssetUrl(url: string | null | undefined): string {
  if (!url) {
    return "";
  }

  return url.startsWith("/") ? `${apiBaseUrl}${url}` : url;
}

/** Zapisuje pobrany plik na dysku użytkownika. */
export function saveDownloadedFile(file: DownloadedFile): void {
  const url = window.URL.createObjectURL(file.blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = file.fileName;
  link.click();
  window.URL.revokeObjectURL(url);
}
