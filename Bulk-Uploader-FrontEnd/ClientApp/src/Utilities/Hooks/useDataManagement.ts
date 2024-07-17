import {useMutation, useQuery, useQueryClient} from "@tanstack/react-query";
import {SimpleGet, SimplePatch} from "../SimpleREST.ts";
import {useMemo} from "react";
import {Hub} from "@mui/icons-material";

export interface ISimpleDmResponse {
    id: string;
    name: string;
}

export function useHubs(options: any) {
    return useQuery<ISimpleDmResponse[]>({
        ...options,
        queryKey: ["hubs"],
        queryFn: () => SimpleGet<ISimpleDmResponse[]>(`/api/dm/hubs`)
    });
}
export function useProjects(hubId: string | null, options: any) {
    return useQuery<ISimpleDmResponse[]>({
        ...options,
        queryKey: ["projects", hubId],
        queryFn: () => SimpleGet<ISimpleDmResponse[]>(`/api/dm/projects?hubId=${hubId}`)
    })
}

export function useTopFolders(hubId: string, projectId: string, options: any) {
    return useQuery<ISimpleDmResponse[]>({
        ...options,
        queryKey: ["topFolders", hubId, projectId],
        queryFn: () => SimpleGet<ISimpleDmResponse[]>(`/api/dm/topFolders?hubId=${hubId}&projectId=${projectId}`)
    })
}

export function useFolderContents(projectId: string, folderId: string, options: any) {
    return useQuery<ISimpleDmResponse[]>({
        ...options,
        queryKey: ["folderContents", projectId, folderId],
        queryFn: () => SimpleGet<ISimpleDmResponse[]>(`/api/dm/folderContent?projectId=${projectId}&folderId=${folderId}`)
    })
}