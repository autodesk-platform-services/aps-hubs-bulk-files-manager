
import {DataGrid, GridColDef} from "@mui/x-data-grid";
import {useNavigate} from "react-router";
import IconButton from "@mui/material/IconButton";
import SyncIcon from "@mui/icons-material/Sync";
import {SimpleDate2} from "../../Components/SimpleDate.tsx";
import {useState} from "react";
import {Button,  Container} from "@mui/material";
import { useBatches } from "../../Utilities/Hooks/useBatches.ts";
import { BatchJob } from "../../Models/BatchJob.ts";
import { BatchStatus } from "../../Components/BatchStatus.tsx";

export const BatchList = () => {
    const navigate = useNavigate()
    const {data: batches, isLoading, refresh} = useBatches();
    const [selected, setSelected] = useState<number[]>([])

    

    const columns: GridColDef<BatchJob>[] = [
        {field: 'name', headerName: 'Name', flex: 2},
        {field: 'type', headerName: 'Type', flex: 1},
        {field: 'jobCount ', headerName: 'Task Count', flex: 1},
        {field: 'created', headerName: 'Created On', flex: 1, renderCell: ({value})=>(<SimpleDate2 date={value}/>)},
        {field: 'queued', headerName: 'Queued On', flex: 1, renderCell: ({value})=>(<SimpleDate2 date={value}/>)},
        {field: 'started', headerName: 'Started On', flex: 1, renderCell: ({value})=>(<SimpleDate2 date={value}/>)},
        {field: 'completed', headerName: 'Completed On', flex: 1, renderCell: ({value})=>(<SimpleDate2 date={value}/>)},

        {
            field: 'id', headerName: 'Action', width: 200, align: "right",
            renderCell: ({row}) => (
                <BatchStatus batchJob= {row}/>
            )
        },

    ]

    return <>
        <div style={{display: 'flex', gap: '1em', alignItems: 'center'}}>
            <h2 style={{flex: 1}}>Active Batches</h2>
            <div>
                {!!selected.length &&
                    <Button onClick={()=>{}} variant={'contained'} color={'error'}>Delete Selected</Button>}
                <IconButton onClick={refresh}>
                    <SyncIcon/>
                </IconButton>
            </div>
        </div>

        <DataGrid
            columns={columns}
            rows={batches ?? []}
            loading={isLoading}
            disableRowSelectionOnClick={true}
            checkboxSelection={true}
            
            rowSelectionModel={selected}
            onRowSelectionModelChange={rowSelectionModel=>setSelected(rowSelectionModel as number[])}
        />
    </>
}


