import {SimpleGet, SimplePost, SimpleDelete} from "../SimpleREST.ts";
import {useMutation, useQuery, useQueryClient} from "@tanstack/react-query";

//Test this in built application
export function useIsAuthenticated() {
    const {data: isAuthenticated, isLoading: isAuthenticatedLoading, refetch} = useQuery({
        queryKey: ['threeLegged/isAuthenticated'],
        queryFn: () => SimpleGet<boolean>(`/api/settings/threeLegged/check`),
        refetchOnWindowFocus: "always"
    });
    
    return {isAuthenticated, isLoading: isAuthenticatedLoading};
}
export function useThreeLegged() {
    const queryClient = useQueryClient();

    // const {data: isAuthenticated, isLoading: isAuthenticatedLoading} = useQuery({
    //     queryKey: ['threeLegged/isAuthenticated'],
    //     queryFn: () => SimpleGet<boolean>(`/api/settings/threeLegged/check`),
    //     // refetchOnWindowFocus: "always"
    // });


    const {data: authUrl, isLoading: isAuthUrlLoading} = useQuery({
        queryKey: ['threeLegged/authUrl'],
        queryFn: () => SimpleGet<string>(`/api/settings/threeLegged/getAuthUrl`),
    });

    const refresh = () => {
        return Promise.all([
            queryClient.invalidateQueries({queryKey: ['threeLegged/isAuthenticated']}),
            queryClient.invalidateQueries({queryKey: ['threeLegged/authUrl']})
        ]);
    }
    const refreshAuthUrl = () => {
        return new Promise((resolve, reject) => {
            refresh().then(()=>{
                setTimeout(resolve, 1000);
            })
        })
    }

    // const processCode = useMutation({
    //     mutationFn: ({code}: { code: string }) => {
    //         debugger;
    //         return SimplePost(`/api/settings/threeLegged`, {code});
    //     },
    //     onSuccess: refresh
    // })

    const logout = useMutation({
        mutationFn: () => {
            return SimpleDelete(`/api/settings/threeLegged`);
        },
        onSuccess: refresh
    })

    return {
        refresh: refreshAuthUrl,
        authUrl,
        // processCode,
        logout,
        isLoading: isAuthUrlLoading
    }
}