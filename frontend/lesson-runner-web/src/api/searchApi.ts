import { apiGet } from "./client";

export type SearchResultKind = "participant" | "group" | "guardian" | "lesson";

export type SearchResult = {
  kind: SearchResultKind;
  id: string;
  title: string;
  subtitle: string | null;
  /** Ścieżka we froncie, pod którą znajduje się zasób. */
  path: string;
};

export type SearchResponse = {
  results: SearchResult[];
};

export function search(query: string): Promise<SearchResponse> {
  return apiGet<SearchResponse>(`/api/search?q=${encodeURIComponent(query)}`);
}
