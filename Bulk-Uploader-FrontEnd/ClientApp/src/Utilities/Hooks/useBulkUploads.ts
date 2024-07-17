import {useMutation, useQuery, useQueryClient} from "@tanstack/react-query";
import {SimpleDelete, SimpleGet, SimplePatch, SimplePost} from "../SimpleREST.ts";

export interface IBulkUpload {
    id: number;
    name: string;
    localPath: string;
    projectId: string;
    folderId: string;
    excludedFolderNames: string;
    excludedFileTypes: string;
    modifyPathScript: string;
    useModifyPathScript: boolean;
    startTime?: Date;
    endTime?: Date;
    status?: string;
    logs?: string;
    proposedFileCount?: number;
    doNotUploadFileCount?: number;
    pendingFileCount?: number;
    successFileCount?: number;
    failedFileCount?: number;
    files?: IBulkUploadFile[]
    autodeskMirrors?: IBulkUploadAutodeskMirror[]
}

export interface IBulkUploadAutodeskMirror{
    folderName: string;
    folderUrl: string;
    folderUrn: string;
    relativeFolderPath: string;
}

export interface IBulkUploadFile{
    id: number;
    bulkUploadId: number;
    sourceFileName: string;
    sourceAbsolutePath: string;
    sourceRelativePath: string;
    targetFileName: string;
    targetRelativePath: string;
    folderUrn: string;
    folderUrl: string;
    webUrl: string;
    versionId: string;
    status: string;
    logs: string;
    createdOn: Date;
    lastModified: Date;
}

export function useBulkUploads(){
    const queryClient = useQueryClient();

    const {data, isLoading, isFetching} = useQuery({
        queryKey: ['bulkUploads'],
        queryFn: () => SimpleGet<IBulkUpload[]>(`/api/bulkUploads` ),
        refetchInterval: 15000
    })

    const refresh = ()=>{
        queryClient.invalidateQueries({queryKey: ['bulkUploads']})
    }

    const mutation = useMutation({
        mutationFn: ({command, bulkUploads}: {command: 'patch' | 'post' | 'delete', bulkUploads?: Partial<IBulkUpload>[]})=>{
            switch(command){
                case 'post': return SimplePost(`/api/bulkUploads`, bulkUploads);
                case 'patch': return SimplePatch(`/api/bulkUploads`, bulkUploads);
                case 'delete': return Promise.all(bulkUploads?.map(bulkUpload=>SimpleDelete(`/api/bulkUploads/${bulkUpload.id}`)) ?? []);
            }
        },
        onSuccess: refresh
    })

    return {data, isLoading: isLoading || isFetching, refresh, mutation}
}

