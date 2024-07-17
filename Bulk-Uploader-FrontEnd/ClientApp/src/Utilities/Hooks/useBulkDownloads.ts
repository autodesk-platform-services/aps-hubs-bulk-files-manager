import {useMutation, useQuery, useQueryClient} from "@tanstack/react-query";
import {SimpleDelete, SimpleGet, SimplePatch, SimplePost} from "../SimpleREST";
import {IBulkUpload} from "./useBulkUploads";

export interface IBulkDownload {
    id: number,
    name: string,
    cloudPath: string,
    localPath: string,
    hubId: string,
    projectId: string,
    apsFolderId: string,
    logs?:string,
    status?:string,
    pendingDownloadCount?:number,
    inProgressDownloadCount?:number,
    successDownloadCount?:number,
    failedDownloadCount?:number,
    files?:IBulkDownloadFile[],
    createdAt: Date,
}

export interface   IBulkDownloadFile {
    id: number,
    bulkDownloadId: number,
    fileName: string,
    sourceFilePath: string,
    destinationFilePath: string,
    itemId: string,
    fileSize: number,
    downloadFileStatus: string,
    status?:string,
    logs: string,
    createdOn: Date,
    lastModified: Date,
}

export function useBulkDownloads() {
    const queryClient = useQueryClient();
    
    const {data, isLoading, isFetching} = useQuery({
        queryKey: ['bulkDownloads'],
        queryFn: () => SimpleGet<IBulkDownload[]>(`/api/bulkDownloads`),
        refetchInterval: 15000
    })

    const refresh = ()=>{
        queryClient.invalidateQueries({queryKey: ['bulkDownloads']})
    }

    const mutation = useMutation({
        mutationFn: ({command, bulkDownloads}: {command: 'patch' | 'post' | 'delete', bulkDownloads?: Partial<IBulkDownload>[]})=>{
            switch(command){
                case 'post': return SimplePost(`/api/download`, bulkDownloads);
                // case 'patch': return SimplePatch(`/api/bulkUploads`, bulkUploads);
                 case 'delete': return Promise.all(bulkDownloads?.map(bulkDownload=>SimpleDelete(`/api/download?id=${bulkDownload.id}`)) ?? []);
                default: return Promise.resolve();
            }
        },
        onSuccess: refresh
    })

    return {data, isLoading: isLoading || isFetching, refresh, mutation}
}