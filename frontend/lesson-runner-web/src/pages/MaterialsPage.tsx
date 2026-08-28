import { useEffect, useMemo, useState } from "react";
import { ExternalLink, File, FileUp, Link as LinkIcon, Pencil, Plus, Trash2, UsersRound } from "lucide-react";
import { createMaterial, deleteMaterial, getMaterials, updateMaterial } from "../api/materialsApi";
import { ApiError, resolveAssetUrl, uploadFile } from "../api/client";
import { Button } from "../components/ui/Button";
import { useAuth } from "../features/auth/AuthContext";
import { useDialogs } from "../features/dialog/DialogContext";
import { formatDateTime } from "../features/groups/datetime";
import { useToast } from "../features/toast/ToastContext";
import type { StoredFile } from "../types/lesson";
import type { Material, MaterialVisibility, UpsertMaterialRequest } from "../types/material";

type MaterialForm = UpsertMaterialRequest & { id: string | null };
type VisibilityFilter = "all" | MaterialVisibility;

const emptyForm: MaterialForm = {
  id: null,
  title: "",
  description: "",
  resourceUrl: "",
  fileName: null,
  contentType: null,
  sizeBytes: null,
  visibility: "staff",
};

export function MaterialsPage() {
  const { isAdmin } = useAuth();
  const { confirm } = useDialogs();
  const toast = useToast();
  const [materials, setMaterials] = useState<Material[]>([]);
  const [form, setForm] = useState<MaterialForm>(emptyForm);
  const [query, setQuery] = useState("");
  const [visibility, setVisibility] = useState<VisibilityFilter>("all");
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [uploading, setUploading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let ignore = false;

    getMaterials()
      .then((items) => {
        if (!ignore) {
          setMaterials(items);
        }
      })
      .catch(() => {
        if (!ignore) {
          setError("Nie udało się pobrać materiałów.");
        }
      })
      .finally(() => {
        if (!ignore) {
          setLoading(false);
        }
      });

    return () => {
      ignore = true;
    };
  }, []);

  const filteredMaterials = useMemo(() => {
    const normalizedQuery = query.trim().toLocaleLowerCase("pl-PL");

    return materials.filter((material) => {
      const matchesVisibility = visibility === "all" || material.visibility === visibility;
      const matchesQuery =
        normalizedQuery.length === 0 ||
        `${material.title} ${material.description} ${material.fileName ?? ""}`
          .toLocaleLowerCase("pl-PL")
          .includes(normalizedQuery);

      return matchesVisibility && matchesQuery;
    });
  }, [materials, query, visibility]);

  function editMaterial(material: Material) {
    setForm({
      id: material.id,
      title: material.title,
      description: material.description,
      resourceUrl: material.resourceUrl,
      fileName: material.fileName,
      contentType: material.contentType,
      sizeBytes: material.sizeBytes,
      visibility: material.visibility,
    });
    setError(null);
  }

  function changeResourceUrl(resourceUrl: string) {
    setForm((current) => ({
      ...current,
      resourceUrl,
      fileName: null,
      contentType: null,
      sizeBytes: null,
    }));
  }

  async function handleFile(file: File | undefined) {
    if (!file) {
      return;
    }

    try {
      setUploading(true);
      setError(null);
      const stored = await uploadFile<StoredFile>(file);
      setForm((current) => ({
        ...current,
        resourceUrl: stored.url,
        fileName: stored.fileName,
        contentType: stored.contentType,
        sizeBytes: stored.sizeBytes,
      }));
      toast.success("Plik został przesłany. Zapisz materiał, aby dodać go do biblioteki.");
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się przesłać pliku.");
    } finally {
      setUploading(false);
    }
  }

  async function saveMaterial() {
    if (!form.title.trim()) {
      setError("Podaj tytuł materiału.");
      return;
    }

    if (!form.resourceUrl.trim()) {
      setError("Podaj link albo prześlij plik.");
      return;
    }

    const request: UpsertMaterialRequest = {
      title: form.title.trim(),
      description: form.description.trim(),
      resourceUrl: form.resourceUrl.trim(),
      fileName: form.fileName,
      contentType: form.contentType,
      sizeBytes: form.sizeBytes,
      visibility: form.visibility,
    };

    try {
      setSaving(true);
      setError(null);
      const saved = form.id
        ? await updateMaterial(form.id, request)
        : await createMaterial(request);

      setMaterials((current) => [saved, ...current.filter((item) => item.id !== saved.id)]);
      setForm(emptyForm);
      toast.success(form.id ? "Materiał został zapisany." : "Materiał został dodany.");
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się zapisać materiału.");
    } finally {
      setSaving(false);
    }
  }

  async function removeMaterial(material: Material) {
    const approved = await confirm({
      title: "Usunąć materiał?",
      description: `Materiał „${material.title}” zniknie z biblioteki personelu.`,
      tone: "danger",
      confirmLabel: "Usuń materiał",
    });

    if (!approved) {
      return;
    }

    try {
      await deleteMaterial(material.id);
      setMaterials((current) => current.filter((item) => item.id !== material.id));
      if (form.id === material.id) {
        setForm(emptyForm);
      }
      toast.success("Materiał został usunięty.");
    } catch (caught) {
      setError(caught instanceof ApiError ? caught.message : "Nie udało się usunąć materiału.");
    }
  }

  return (
    <section className="page-section">
      <div className="page-header">
        <div>
          <span className="eyebrow">{isAdmin ? "Administracja i instruktorzy" : "Panel instruktora"}</span>
          <h1>Materiały</h1>
          <p>
            {isAdmin
              ? "Udostępniaj pliki i linki administracji albo całemu zespołowi instruktorskiemu."
              : "Pliki i linki udostępnione przez administrację dla zespołu instruktorskiego."}
          </p>
        </div>
        {isAdmin ? (
          <Button variant="secondary" onClick={() => setForm(emptyForm)}>
            <Plus className="button-icon" aria-hidden="true" />
            Nowy materiał
          </Button>
        ) : null}
      </div>

      {error ? <div className="list-state list-state-error" role="alert">{error}</div> : null}

      <div className="materials-toolbar">
        <label>
          <span>Szukaj</span>
          <input
            type="search"
            placeholder="Tytuł, opis lub nazwa pliku"
            value={query}
            onChange={(event) => setQuery(event.target.value)}
          />
        </label>
        {isAdmin ? (
          <label>
            <span>Widoczność</span>
            <select value={visibility} onChange={(event) => setVisibility(event.target.value as VisibilityFilter)}>
              <option value="all">Wszystkie materiały</option>
              <option value="staff">Administracja i instruktorzy</option>
              <option value="admin">Tylko administracja</option>
            </select>
          </label>
        ) : null}
      </div>

      {loading ? <div className="list-state" role="status">Ładowanie materiałów…</div> : null}

      {!loading ? (
        <div className={isAdmin ? "materials-layout" : "materials-layout materials-layout-readonly"}>
          <div className="material-list">
            {filteredMaterials.length === 0 ? (
              <div className="empty-panel">
                <h2>{materials.length === 0 ? "Biblioteka jest jeszcze pusta" : "Brak pasujących materiałów"}</h2>
                <p>
                  {isAdmin && materials.length === 0
                    ? "Dodaj pierwszy link albo plik i wybierz, kto ma go widzieć."
                    : "Zmień wyszukiwaną frazę lub filtr."}
                </p>
              </div>
            ) : (
              filteredMaterials.map((material) => (
                <article className="material-card" key={material.id}>
                  <div className="material-card-icon" aria-hidden="true">
                    {material.fileName ? <File /> : <LinkIcon />}
                  </div>
                  <div className="material-card-content">
                    <div className="material-card-heading">
                      <div>
                        <h2>{material.title}</h2>
                        <span className={`material-visibility material-visibility-${material.visibility}`}>
                          <UsersRound size={14} aria-hidden="true" />
                          {material.visibility === "admin" ? "Tylko administracja" : "Administracja i instruktorzy"}
                        </span>
                      </div>
                      {isAdmin ? (
                        <div className="material-admin-actions">
                          <Button variant="ghost" onClick={() => editMaterial(material)} aria-label={`Edytuj ${material.title}`}>
                            <Pencil className="button-icon" aria-hidden="true" />
                            Edytuj
                          </Button>
                          <Button variant="ghost" onClick={() => void removeMaterial(material)} aria-label={`Usuń ${material.title}`}>
                            <Trash2 className="button-icon" aria-hidden="true" />
                            Usuń
                          </Button>
                        </div>
                      ) : null}
                    </div>
                    {material.description ? <p>{material.description}</p> : null}
                    <div className="material-card-footer">
                      <span>
                        {material.fileName ?? "Link zewnętrzny"}
                        {material.sizeBytes !== null ? ` · ${formatBytes(material.sizeBytes)}` : ""}
                        {` · aktualizacja ${formatDateTime(material.updatedAt)}`}
                      </span>
                      <a
                        className="button button-secondary"
                        href={resolveAssetUrl(material.resourceUrl)}
                        target="_blank"
                        rel="noreferrer"
                      >
                        <ExternalLink className="button-icon" aria-hidden="true" />
                        {material.fileName ? "Otwórz plik" : "Otwórz link"}
                      </a>
                    </div>
                  </div>
                </article>
              ))
            )}
          </div>

          {isAdmin ? (
            <fieldset className="editor-fieldset material-editor">
              <legend>{form.id ? "Edytuj materiał" : "Nowy materiał"}</legend>
              <label className="form-field">
                <span>Tytuł</span>
                <input
                  maxLength={200}
                  value={form.title}
                  onChange={(event) => setForm((current) => ({ ...current, title: event.target.value }))}
                />
              </label>
              <label className="form-field">
                <span>Opis</span>
                <textarea
                  rows={4}
                  maxLength={2000}
                  value={form.description}
                  onChange={(event) => setForm((current) => ({ ...current, description: event.target.value }))}
                />
              </label>
              <label className="form-field">
                <span>Link</span>
                <input
                  type="url"
                  placeholder="https://…"
                  value={form.resourceUrl}
                  onChange={(event) => changeResourceUrl(event.target.value)}
                />
              </label>
              <div className="material-resource-divider"><span>albo</span></div>
              <label className="material-file-picker">
                <FileUp aria-hidden="true" />
                <span>{uploading ? "Przesyłanie…" : "Wybierz plik"}</span>
                <input
                  type="file"
                  disabled={uploading}
                  onChange={(event) => void handleFile(event.target.files?.[0])}
                />
              </label>
              {form.fileName ? (
                <p className="material-selected-file">
                  <File size={16} aria-hidden="true" />
                  {form.fileName} {form.sizeBytes !== null ? `(${formatBytes(form.sizeBytes)})` : ""}
                </p>
              ) : null}
              <label className="form-field">
                <span>Widoczność</span>
                <select
                  value={form.visibility}
                  onChange={(event) =>
                    setForm((current) => ({ ...current, visibility: event.target.value as MaterialVisibility }))
                  }
                >
                  <option value="staff">Administracja i instruktorzy</option>
                  <option value="admin">Tylko administracja</option>
                </select>
              </label>
              <div className="button-row">
                <Button onClick={() => void saveMaterial()} disabled={saving || uploading}>
                  {saving ? "Zapisywanie…" : form.id ? "Zapisz zmiany" : "Dodaj materiał"}
                </Button>
                {form.id ? (
                  <Button variant="secondary" onClick={() => setForm(emptyForm)}>
                    Anuluj
                  </Button>
                ) : null}
              </div>
            </fieldset>
          ) : null}
        </div>
      ) : null}
    </section>
  );
}

function formatBytes(bytes: number): string {
  if (bytes < 1024) {
    return `${bytes} B`;
  }

  if (bytes < 1024 * 1024) {
    return `${Math.round(bytes / 1024)} KB`;
  }

  return `${(bytes / (1024 * 1024)).toFixed(1).replace(".", ",")} MB`;
}
