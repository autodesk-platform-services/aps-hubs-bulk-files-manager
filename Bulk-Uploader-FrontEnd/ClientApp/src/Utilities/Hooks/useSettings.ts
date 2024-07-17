import {useMutation, useQuery, useQueryClient} from "@tanstack/react-query";
import {SimpleGet, SimplePatch} from "../SimpleREST.ts";
import {useMemo} from "react";

export interface ISetting{
    settingId: number;
    key: string;
    value: string;
}
export function useSettings(){
    const queryClient = useQueryClient();

    const {data, isLoading, isFetching} = useQuery({
        queryKey: ['settings'],
        queryFn: () => SimpleGet<ISetting[]>(`/api/settings` )
    })

    const refresh = ()=>{
        queryClient.invalidateQueries({queryKey: ['settings']})
    }

    const mutation = useMutation({
        mutationFn: ({command, settings}: {command: 'patch', settings?: ISetting[]})=>{
            switch(command){
                case 'patch': return SimplePatch(`/api/settings`, settings);
            }
        },
        onSuccess: refresh
    })

    const dataHash = useMemo(()=>{
        return data?.reduce((acc: {[key: string]: ISetting}, val)=>({...acc, [val.key]: val}), {}) ?? {}
    }, [data])

    return {data, dataHash, isLoading: isLoading || isFetching, refresh, mutation}
}