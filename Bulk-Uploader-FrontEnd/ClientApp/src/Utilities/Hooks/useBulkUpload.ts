import {useMutation, useQuery, useQueryClient} from "@tanstack/react-query";
import {SimpleGet, SimplePost} from "../SimpleREST.ts";
import {IBulkUpload, IBulkUploadFile} from "./useBulkUploads.ts";

export function useBulkUpload(bulkUploadId: number | string){
    const queryClient = useQueryClient();

    const {data, isLoading, isFetching} = useQuery({
        queryKey: ['bulkUpload', {bulkUploadId}] as [string, {bulkUploadId: string | number}],
        queryFn: ({queryKey: [, {bulkUploadId}]}) => {
            return SimpleGet<IBulkUpload>(`/api/bulkUploads/${bulkUploadId}` )
        },
        refetchInterval: 15000
    })

    const refresh = ()=>{
        queryClient.invalidateQueries({queryKey: ['bulkUpload', {bulkUploadId}]})
        queryClient.invalidateQueries({queryKey: ['bulkUploads']})
    }

    const filesMutation = useMutation({
        mutationFn: ({command, bulkUploadId, files}: {command: 'repeatFiles', bulkUploadId: number, files?: Partial<IBulkUploadFile>[]})=>{
            switch(command){
                case 'repeatFiles': return SimplePost(`/api/bulkUploads/${bulkUploadId}/bulkUploadFiles`, files);
            }
        },
        onSuccess: refresh
    })

    return {data, isLoading: isLoading || isFetching, refresh, filesMutation}
}