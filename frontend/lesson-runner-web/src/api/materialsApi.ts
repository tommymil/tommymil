import { apiDelete, apiGet, apiPost, apiPut } from "./client";
import type { Material, UpsertMaterialRequest } from "../types/material";

export function getMaterials(): Promise<Material[]> {
  return apiGet<Material[]>("/api/materials");
}

export function createMaterial(request: UpsertMaterialRequest): Promise<Material> {
  return apiPost<UpsertMaterialRequest, Material>("/api/materials", request);
}

export function updateMaterial(id: string, request: UpsertMaterialRequest): Promise<Material> {
  return apiPut<UpsertMaterialRequest, Material>(`/api/materials/${id}`, request);
}

export function deleteMaterial(id: string): Promise<void> {
  return apiDelete(`/api/materials/${id}`);
}
