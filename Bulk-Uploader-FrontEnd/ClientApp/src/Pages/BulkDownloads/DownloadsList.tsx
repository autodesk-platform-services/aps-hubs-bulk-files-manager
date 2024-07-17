import {IBulkUpload, useBulkUploads} from "../..//Utilities/Hooks/useBulkUploads.ts";
import {DataGrid, GridColDef} from "@mui/x-data-grid";
import {ColoredStatusButton} from "../../Components/ColoredStatusButton.tsx";
import IconButton from "@mui/material/IconButton";
import SyncIcon from "@mui/icons-material/Sync";
import {SimpleDate} from "../../Components/SimpleDate.tsx";
import {useState} from "react";
import {Button} from "@mui/material";
import {IBulkDownload, useBulkDownloads} from "../../Utilities/Hooks/useBulkDownloads";
import {useNavigate} from "react-router";

export const DownloadsList = () => {
    const navigate = useNavigate()
    const {data: bulkDownloads, isLoading, mutation, refresh} = useBulkDownloads();
    const [selected, setSelected] = useState<number[]>([])
    
    function deleteSelected(){
        const confirmation = window.confirm(`Are you sure you want to delete these ${selected.length} download records? \r\n\r\nThis will not remove any files that have already been downloaded.`)
        const downloads = bulkDownloads?.filter(x=>selected.indexOf(x.id) > -1) ?? [];

        if(confirmation && downloads.length){
            mutation.mutateAsync({command: 'delete', bulkDownloads: downloads})
        }
    }
    
    const columns: GridColDef<IBulkDownload>[] = [
        {field: 'name', headerName: 'Name', flex: 2},
        {field: 'status', headerName: 'Status', width: 100},
        {field: 'localPath', headerName: 'Local Path', flex: 1},
        {field: 'projectId', headerName: 'Project Id', flex:1},
        {field: 'apsFolderId', headerName: 'Folder Id', flex: 1},
        {
            field: 'pendingDownloadCount', headerName: 'Pending', width: 75,
            renderCell: ({row}) => (
                <ColoredStatusButton
                    color={'info'}
                    count={row.pendingDownloadCount}
                    path={`/downloads/${row.id}/status/Pending`}
                />
            )
        },
        {
            field: 'inProgressDownloadCount', headerName: 'Processing', width: 90,
            renderCell: ({row}) => (
                <ColoredStatusButton
                    color={'secondary'}
                    count={row.inProgressDownloadCount}
                    path={`/downloads/${row.id}/status/Pending`}
                />
            )
        },
        {
            field: 'successDownloadCount', headerName: 'Success', width: 75,
            renderCell: ({row}) => (
                <ColoredStatusButton    
                    color={'success'}
                    count={row.successDownloadCount}
                    path={`/downloads/${row.id}/status/Success`}
                />
            )
        },
        {
            field: 'failedDownloadCount', headerName: 'Failed', width: 75,
            renderCell: ({row}) => (
                <ColoredStatusButton
                    color={'error'}
                    count={row.failedDownloadCount}
                    path={`/downloads/${row.id}/status/Failed`}
                />
            )
        },
    ]

    return <>
        <div style={{display: 'flex', gap: '1em', alignItems: 'center'}}>
            <h2 style={{flex: 1}}>Downloads History</h2>
            <div>
                {!!selected.length &&
                    <Button onClick={deleteSelected} variant={'contained'} color={'error'}>Delete Selected</Button>}
                <IconButton onClick={refresh}>
                    <SyncIcon/>
                </IconButton>
            </div>
        </div>

        <DataGrid
            columns={columns}
            rows={bulkDownloads ?? []}
            loading={isLoading}
            disableRowSelectionOnClick={true}
            checkboxSelection={true}
            onRowClick={({row}) => navigate(`/downloads/${row.id}`)}
            rowSelectionModel={selected}
            onRowSelectionModelChange={rowSelectionModel=>setSelected(rowSelectionModel as number[])}
        />
    </>
}