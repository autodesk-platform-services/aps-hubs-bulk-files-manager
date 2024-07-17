import { useQuery } from "@tanstack/react-query";
import { BatchUiEntry } from "../Components/BatchUiEntry";
import { BatchEntry } from "../Models/BatchEntry";
import { SimpleGet } from "../Utilities/SimpleREST";




export const SelectionsPage = () => {


    const { data, isLoading, isFetching } = useQuery({
        queryKey: ['batch'],
        queryFn: () => SimpleGet<BatchEntry[]>(`/api/batch`)
    })

    if (isLoading) {
        return <div key="ke">Loading...</div>
    }

    if (isFetching) {
        return <div key="ke2">Loading...</div>
    }

   // const items: ReadonlyArray<BatchEntry> [...data]
    return (
        (
            <div key="ke4">
                <h2>Operation Selections</h2>
    
                <div
                    style={{
                        padding: "1em",
                        display: "flex",
                        flexDirection: "column",
                        gap: "1em",
                    }}
                >
    
                    {data?.map(p => (<BatchUiEntry key={p.key} entry={p}/>))}
                </div>
            </div>
    
        )

    );
};
