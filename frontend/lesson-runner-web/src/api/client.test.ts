import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import {
  ApiError,
  apiDelete,
  apiGet,
  apiPost,
  resolveAssetUrl,
  setAuthToken,
  setUnauthorizedHandler,
  uploadFile,
} from "./client";

type FetchMock = ReturnType<typeof vi.fn>;

function mockResponse(status: number, body: unknown = {}) {
  return {
    ok: status >= 200 && status < 300,
    status,
    json: () => Promise.resolve(body),
  } as Response;
}

describe("api client", () => {
  let fetchMock: FetchMock;

  beforeEach(() => {
    fetchMock = vi.fn();
    vi.stubGlobal("fetch", fetchMock);
    setAuthToken(null);
    setUnauthorizedHandler(null);
  });

  afterEach(() => {
    vi.unstubAllGlobals();
  });

  it("does not send Authorization header when no token is set", async () => {
    fetchMock.mockResolvedValue(mockResponse(200, { ok: true }));

    await apiGet("/api/lessons");

    const [, init] = fetchMock.mock.calls[0];
    expect((init.headers as Record<string, string>).Authorization).toBeUndefined();
  });

  it("attaches a Bearer token when set", async () => {
    fetchMock.mockResolvedValue(mockResponse(200, []));
    setAuthToken("abc123");

    await apiGet("/api/lessons");

    const [, init] = fetchMock.mock.calls[0];
    expect((init.headers as Record<string, string>).Authorization).toBe("Bearer abc123");
  });

  it("sends JSON body and content-type on POST", async () => {
    fetchMock.mockResolvedValue(mockResponse(201, {}));

    await apiPost("/api/lessons", { title: "X" });

    const [, init] = fetchMock.mock.calls[0];
    expect(init.method).toBe("POST");
    expect((init.headers as Record<string, string>)["Content-Type"]).toBe("application/json");
    expect(init.body).toBe(JSON.stringify({ title: "X" }));
  });

  it("invokes the unauthorized handler and throws ApiError on 401", async () => {
    fetchMock.mockResolvedValue(mockResponse(401));
    const onUnauthorized = vi.fn();
    setUnauthorizedHandler(onUnauthorized);

    await expect(apiGet("/api/lessons")).rejects.toMatchObject({ status: 401 });
    expect(onUnauthorized).toHaveBeenCalledOnce();
  });

  it("throws ApiError with the status for other failures", async () => {
    fetchMock.mockResolvedValue(mockResponse(500));

    const error = await apiGet("/api/lessons").catch((caught) => caught);
    expect(error).toBeInstanceOf(ApiError);
    expect((error as ApiError).status).toBe(500);
  });

  it("returns undefined for 204 responses", async () => {
    fetchMock.mockResolvedValue({ ok: true, status: 204 } as Response);

    await expect(apiDelete("/api/lessons/1")).resolves.toBeUndefined();
  });

  it("uploads a file as FormData without a JSON content type", async () => {
    const stored = { url: "/uploads/x.png", fileName: "x.png", contentType: "image/png", sizeBytes: 10 };
    fetchMock.mockResolvedValue(mockResponse(200, stored));
    setAuthToken("abc123");
    const file = new File(["data"], "x.png", { type: "image/png" });

    await expect(uploadFile(file)).resolves.toEqual(stored);

    const [url, init] = fetchMock.mock.calls[0];
    expect(url).toContain("/api/files");
    expect(init.method).toBe("POST");
    expect(init.body).toBeInstanceOf(FormData);
    expect((init.headers as Record<string, string>)["Content-Type"]).toBeUndefined();
    expect((init.headers as Record<string, string>).Authorization).toBe("Bearer abc123");
  });

  it("surfaces the backend error message when an upload fails", async () => {
    fetchMock.mockResolvedValue(mockResponse(400, { error: "Niedozwolony typ pliku." }));
    const file = new File(["data"], "x.txt", { type: "text/plain" });

    const error = await uploadFile(file).catch((caught) => caught);
    expect(error).toBeInstanceOf(ApiError);
    expect((error as ApiError).message).toBe("Niedozwolony typ pliku.");
  });

  it("resolves relative asset urls against the API base and leaves absolute urls untouched", () => {
    expect(resolveAssetUrl("/uploads/x.png")).toBe("http://localhost:5000/uploads/x.png");
    expect(resolveAssetUrl("https://scratch.mit.edu")).toBe("https://scratch.mit.edu");
    expect(resolveAssetUrl(null)).toBe("");
  });
});
