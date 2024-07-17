import {  useQuery, useQueryClient } from "@tanstack/react-query";
import { SimpleGet } from "../SimpleREST.ts";

import { BatchJob } from "../../Models/BatchJob.ts";


export function useBatches() {
    const queryClient = useQueryClient();

    const { data, isLoading, isFetching } = useQuery({
        queryKey: ['batches'],
        queryFn: () => SimpleGet<BatchJob[]>(`/api/batch/active`)
    });

    const refresh = () => {
        queryClient.invalidateQueries({ queryKey: ['batches'] });
    };

    

    return { data, isLoading: isLoading || isFetching, refresh };
}
