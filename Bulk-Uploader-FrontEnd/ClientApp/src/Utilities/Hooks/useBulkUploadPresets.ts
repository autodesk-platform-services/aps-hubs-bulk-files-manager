import {useMutation, useQuery, useQueryClient} from "@tanstack/react-query";
import {SimpleDelete, SimpleGet, SimplePatch, SimplePost} from "../SimpleREST.ts";

export interface IBulkUploadPreset{
    id: number;
    name: string;
    excludedFolderNames: string;
    excludedFileTypes: string;
    modifyPathScript: string;
    useModifyPathScript: boolean;
}
export function useBulkUploadPresets(options: any){
    const queryClient = useQueryClient();

    const {data, isLoading, isFetching} = useQuery<IBulkUploadPreset[]>({
        ...options,
        queryKey: ['bulkUploadPresets'],
        queryFn: () => SimpleGet<IBulkUploadPreset[]>(`/api/settings/bulkUploadPresets` )
    })

    const refresh = ()=>{
        queryClient.invalidateQueries({queryKey: ['bulkUploadPresets']})
    }

    const mutation = useMutation({
        mutationFn: ({command, presets}: {command: 'patch' | 'post' | 'delete', presets?: Partial<IBulkUploadPreset>[]})=>{
            switch(command){
                case 'post': return SimplePost(`/api/settings/bulkUploadPresets`, presets);
                case 'patch': return SimplePatch(`/api/settings/bulkUploadPresets`, presets);
                case 'delete':
                    return Promise.all(presets?.map(preset=>SimpleDelete(`/api/settings/bulkUploadPresets/${preset.id}`)) ?? [])
            }
        },
        onSuccess: refresh
    })

    return {data, isLoading: isLoading || isFetching, refresh, presetMutation: mutation}
}