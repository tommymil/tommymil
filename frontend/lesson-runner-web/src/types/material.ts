export type MaterialVisibility = "admin" | "staff";

export type Material = {
  id: string;
  title: string;
  description: string;
  resourceUrl: string;
  fileName: string | null;
  contentType: string | null;
  sizeBytes: number | null;
  visibility: MaterialVisibility;
  createdAt: string;
  updatedAt: string;
};

export type UpsertMaterialRequest = {
  title: string;
  description: string;
  resourceUrl: string;
  fileName: string | null;
  contentType: string | null;
  sizeBytes: number | null;
  visibility: MaterialVisibility;
};
