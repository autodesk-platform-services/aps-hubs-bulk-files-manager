import {useNavigate, useParams} from "react-router";
import {IBulkDownloadFile, useBulkDownloads} from "../../Utilities/Hooks/useBulkDownloads";
import {useMemo, useState} from "react";
import {useBulkDownload} from "../../Utilities/Hooks/useBulkDownload";
import {DataGrid, GridColDef} from "@mui/x-data-grid";
import Button from "@mui/material/Button";
import {BulkDownloadSingleHeader} from "./BulkDownloadSingleHeader.tsx";
import {useBulkDownloadStatus} from "../../Utilities/Hooks/useBulkDownloadStatus.ts";

export function bytesToSize(bytes?: number) {
    const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
    if (bytes == 0) return '0 Byte';
    const i = Math.round(Math.floor(Math.log(bytes || 0) / Math.log(1024)));
    return Math.round((bytes || 0) / Math.pow(1024, i)) + ' ' + sizes[i];
}
export const DownloadSingleStatus = () => {
    const {bulkDownloadId, status} = useParams() as {bulkDownloadId: string, status: string};
    const navigate = useNavigate();
    const {data: bulkDownload, mutation, refresh} = useBulkDownload(bulkDownloadId);

    
    const [selected, setSelected] = useState<number[]>([]);
    const [paginationModel, setPaginationModel] = useState({
        pageSize: 25,
        page: 0,
    });

    const {data: rows, isLoading, refetch: refreshStatus} = useBulkDownloadStatus(
        {bulkDownloadId, status, count: paginationModel.pageSize, skip: paginationModel.page * paginationModel.pageSize});

    // const rows = useMemo(() => {
    //     return bulkDownload?.files?.filter(x=> x.status === status) ?? []
    // }, [bulkDownload, status])
    
    const columns: GridColDef<IBulkDownloadFile>[] = [
        {field: 'fileName', headerName: 'File Name', flex:1},
        {field: 'sourceFilePath', headerName: 'Source File Path', flex:1},
        {field: 'destinationFilePath', headerName: 'Download Path', flex:1},
        {field: 'fileSize', headerName: 'File Size', sortable: false, width: 100, type: 'number',
            renderCell: ({row})=>bytesToSize(row.fileSize)},
        {field: 'logs', headerName: 'Logs', flex:1},
        {field: 'createdOn', headerName: 'Created At', flex:1},
        {field: 'lastModified', headerName: 'Last Modified', flex:1}
    ]



    function restartFiles() {
        const confirmation = window.confirm("Are you sure you want to restart the downloads for these files?");

        if (confirmation && bulkDownload) {
            mutation.mutateAsync({
                command: 'repeatFiles', 
                bulkDownloadId: bulkDownload.id,
                files: rows?.filter(x=>selected.indexOf(x.id) > -1) ?? []
            })
                .then(() => {
                    navigate('/downloads')
                })
        }
    }


    const rowCount = useMemo(()=>{
        switch(status){
            case "Pending": return bulkDownload?.pendingDownloadCount ?? 0;
            case "InProgress": return bulkDownload?.inProgressDownloadCount ?? 0;
            case "Success": return bulkDownload?.successDownloadCount ?? 0;
            case "Failed": return bulkDownload?.failedDownloadCount ?? 0;
            default: return 0;
        }
    }, [bulkDownload, status])
    
    return (
    <>
        <BulkDownloadSingleHeader bulkDownload={bulkDownload} refresh={refresh}/>
        <div>
            <Button disabled={!selected.length} onClick={()=>restartFiles()} variant={'contained'} color={'success'} sx={{mb: 1}}>Download selected files</Button>
            <DataGrid
                density={'compact'}
                rowCount={rowCount}
                paginationMode={'server'}
                paginationModel={paginationModel}
                onPaginationModelChange={setPaginationModel}
                columns={columns}
                rows={rows}
                loading={isLoading}
                disableRowSelectionOnClick={true}
                checkboxSelection={true}
                rowSelectionModel={selected}
                onRowSelectionModelChange={rowSelectionModel=>setSelected(rowSelectionModel as number[])}
            />
        </div>
    </>
    )
}