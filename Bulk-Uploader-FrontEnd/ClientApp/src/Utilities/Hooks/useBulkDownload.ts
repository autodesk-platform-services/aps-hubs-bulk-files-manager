import {useMutation, useQuery, useQueryClient} from "@tanstack/react-query";
import {SimpleGet, SimplePost} from "../SimpleREST";
import {IBulkDownload, IBulkDownloadFile} from "./useBulkDownloads";


export function useBulkDownload(bulkDownloadId: number | string){
    const queryClient = useQueryClient();
    
    const {data, isLoading, isFetching} = useQuery({
        queryKey: ['bulkDownload', {bulkDownloadId}] as [string, {bulkDownloadId: string | number}],
        queryFn: ({queryKey: [, {bulkDownloadId}]}) => {
            return SimpleGet<IBulkDownload>(`/api/downloads/${bulkDownloadId}`)
        },
        refetchInterval: 15000
    })

    const refresh = ()=>{
        queryClient.invalidateQueries({queryKey: ['bulkDownload', {bulkDownloadId}]})
        queryClient.invalidateQueries({queryKey: ['bulkDownloads']})
    }

    const mutation = useMutation({
        mutationFn: ({command, bulkDownloadId, files}: {command: 'repeatFiles', bulkDownloadId: number, files?: Partial<IBulkDownloadFile>[]})=>{
            switch(command){
                case 'repeatFiles': return SimplePost(`/api/bulkDownloads/${bulkDownloadId}/bulkDownloadFiles`, files);
            }
        },
        onSuccess: refresh
    })
    

    return {data, isLoading: isLoading || isFetching, refresh, mutation}
}