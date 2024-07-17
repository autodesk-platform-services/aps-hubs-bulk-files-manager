import { useQuery } from "@tanstack/react-query";
import { SimpleGet  } from "../SimpleREST.ts";
import {IBulkUploadFile} from "./useBulkUploads.ts";
export function useBulkUploadStatus({bulkUploadId, status, count, skip}: {count?: number, skip?: number, bulkUploadId: number | string, status: string}){

    const {data, isLoading, isFetching, refetch} = useQuery({
        queryKey: [`bulkUploads/singleStatus`, {bulkUploadId, status, count, skip}] as [string, {count?: number, skip?: number, bulkUploadId: string | number, status: string}],
        queryFn: () => SimpleGet<IBulkUploadFile[]>(`/api/bulkUploads/${bulkUploadId}/status/${status}`, {params: {count, skip}} ),
        cacheTime: 15000,
        initialData: [],
        refetchOnMount: true
    })

    return {data, isLoading: isLoading || isFetching, refetch}
}

