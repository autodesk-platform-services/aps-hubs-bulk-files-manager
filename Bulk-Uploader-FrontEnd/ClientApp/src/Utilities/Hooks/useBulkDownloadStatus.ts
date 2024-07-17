import { useQuery } from "@tanstack/react-query";
import { SimpleGet  } from "../SimpleREST.ts";
import {IBulkDownloadFile} from "./useBulkDownloads.ts";
export function useBulkDownloadStatus({bulkDownloadId, status, count, skip}: {count?: number, skip?: number, bulkDownloadId: number | string, status: string}){

    const {data, isLoading, isFetching, refetch} = useQuery({
        queryKey: [`downloads/singleStatus`, {bulkDownloadId, status, count, skip}] as [string, {count?: number, skip?: number, bulkDownloadId: string | number, status: string}],
        queryFn: () => SimpleGet<IBulkDownloadFile[]>(`/api/downloads/${bulkDownloadId}/status/${status}`, {params: {count, skip}} ),
        cacheTime: 15000,
        initialData: [],
        refetchOnMount: true
    })

    return {data, isLoading: isLoading || isFetching, refetch}
}

