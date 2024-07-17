import {IBulkUpload, useBulkUploads} from "../../Utilities/Hooks/useBulkUploads.ts";
import {DataGrid, GridColDef} from "@mui/x-data-grid";
import {ColoredStatusButton} from "../../Components/ColoredStatusButton.tsx";
import {useNavigate} from "react-router";
import IconButton from "@mui/material/IconButton";
import SyncIcon from "@mui/icons-material/Sync";
import {SimpleDate} from "../../Components/SimpleDate.tsx";
import {useState} from "react";
import {Button} from "@mui/material";

export const BulkUploadList = () => {
    const navigate = useNavigate()
    const {data: bulkUploads, isLoading, mutation, refresh} = useBulkUploads();
    const [selected, setSelected] = useState<number[]>([])

    function deleteSelected(){
        const confirmation = window.confirm(`Are you sure you want to delete these ${selected.length} uploads? \r\n\r\nThis will not remove any files that have already been uploaded.`)
        const uploads = bulkUploads?.filter(x=>selected.indexOf(x.id) > -1) ?? [];

        if(confirmation && uploads.length){
            mutation.mutateAsync({command: 'delete', bulkUploads: uploads})
        }
    }

    const columns: GridColDef<IBulkUpload>[] = [
        {field: 'name', headerName: 'Name', flex: 2},
        {field: 'localPath', headerName: 'Local Path', flex: 1},
        {field: 'projectId', headerName: 'Project ID', flex: 1},
        {field: 'folderId', headerName: 'Folder ID', flex: 1},

        {
            field: 'proposedFileCount', headerName: 'Proposed', width: 75,
            renderCell: ({row}) => (
                <ColoredStatusButton
                    color={'secondary'}
                    count={row.proposedFileCount}
                    path={`/bulkUploads/${row.id}/status/Proposed`}
                />
            )
        },

        {
            field: 'doNotUploadFileCount', headerName: 'Skip', width: 75,
            renderCell: ({row}) => (
                <ColoredStatusButton
                    color={'warning'}
                    count={row.doNotUploadFileCount}
                    path={`/bulkUploads/${row.id}/status/DoNotUpload`}
                />
            )
        },

        {
            field: 'pendingFileCount', headerName: 'Pending', width: 75,
            renderCell: ({row}) => (
                <ColoredStatusButton
                    color={'info'}
                    count={row.pendingFileCount}
                    path={`/bulkUploads/${row.id}/status/Pending`}
                />
            )
        },
        {
            field: 'successFileCount', headerName: 'Success', width: 75,
            renderCell: ({row}) => (
                <ColoredStatusButton
                    color={'success'}
                    count={row.successFileCount}
                    path={`/bulkUploads/${row.id}/status/Success`}
                />
            )
        },
        {
            field: 'failedFileCount', headerName: 'Failed', width: 75,
            renderCell: ({row}) => (
                <ColoredStatusButton
                    color={'error'}
                    count={row.failedFileCount}
                    path={`/bulkUploads/${row.id}/status/Failed`}
                />
            )
        },

        {field: 'startTime', headerName: 'Start', flex: 1, renderCell: ({value})=>(<SimpleDate date={value}/>)},
        {field: 'endTime', headerName: 'End', flex: 1, renderCell: ({value})=>(<SimpleDate date={value}/>)}
    ]

    return <>
        <div style={{display: 'flex', gap: '1em', alignItems: 'center'}}>
            <h2 style={{flex: 1}}>Bulk Upload History</h2>
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
            rows={bulkUploads ?? []}
            loading={isLoading}
            disableRowSelectionOnClick={true}
            checkboxSelection={true}
            onRowClick={({row}) => navigate(`/bulkUploads/${row.id}`)}
            rowSelectionModel={selected}
            onRowSelectionModelChange={rowSelectionModel=>setSelected(rowSelectionModel as number[])}
        />
    </>
}


