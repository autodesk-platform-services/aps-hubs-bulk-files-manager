import {useParams} from "react-router";
import {useBulkUpload} from "../../Utilities/Hooks/useBulkUpload.ts";
import {DataGrid, GridColDef} from "@mui/x-data-grid";
import {useMemo, useState} from "react";
import {IBulkUploadFile} from "../../Utilities/Hooks/useBulkUploads.ts";
import {BulkUploadSingleHeader} from "./BulkUploadSingleHeader.tsx";
import {SimpleDate} from "../../Components/SimpleDate.tsx";
import Button from "@mui/material/Button";
import {useBulkUploadStatus} from "../../Utilities/Hooks/useBulkUploadStatus.ts";

export const BulkUploadSingleStatus = () => {
    const {bulkUploadId, status} = useParams() as { bulkUploadId: string, status: string };
    const {
        data: bulkUpload,
        isLoading: uploadIsLoading,
        refresh,
        filesMutation} = useBulkUpload(bulkUploadId)
    const [selected, setSelected] = useState<number[]>([])
    const [paginationModel, setPaginationModel] = useState({
        pageSize: 25,
        page: 0,
    });

    const {data: rows, isLoading, refetch: refreshStatus} = useBulkUploadStatus(
        {bulkUploadId, status, count: paginationModel.pageSize, skip: paginationModel.page * paginationModel.pageSize});
    // const rows = useMemo(() => {
    //     return bulkUpload?.files?.filter(x => x.status === status) ?? []
    // }, [bulkUpload, status])

    function restartFiles(){
        if(!bulkUpload || !bulkUpload?.files?.length || !selected.length) return;
        const confirmation = window.confirm("Are you sure you want to repeat the processing on these jobs? This may cause the file to be uploaded multiple times");

        if(confirmation){
            const files = bulkUpload.files.filter(x=>selected.indexOf(x.id) > -1);
            filesMutation.mutate({
                command: 'repeatFiles',
                bulkUploadId: bulkUpload.id,
                files
            })
        }
    }

    const columns: GridColDef<IBulkUploadFile>[] = [
        {field: 'sourceFileName', sortable: false, headerName: "File Name", flex: 1, renderCell: ({row, value})=>(
                <span title={row.versionId}>{value}</span>
            )},
        {field: 'sourceRelativePath', sortable: false, headerName: "Path", flex: 1},
        {field: 'targetFileName', sortable: false, headerName: "Target File Name", flex: 1, renderCell: ({row, value})=>(
            row.webUrl ? <a href={row.webUrl}>{value}</a> : value
            )},
        {field: 'targetRelativePath', sortable: false, headerName: "Target Path", flex: 1},
        {field: 'folderUrn', sortable: false, headerName: "Cloud Folder", flex: 1},
        {field: 'status', sortable: false, headerName: "Status"},
        {field: 'logs', sortable: false, headerName: "Logs", flex: 1},
        {field: 'createdOn', sortable: false, headerName: "Created On", renderCell: ({value})=>(<SimpleDate date={value}/>)},
        {field: 'lastModified', sortable: false, headerName: "Last Modified", renderCell: ({value})=>(<SimpleDate date={value}/>)},
    ]

    const rowCount = useMemo(()=>{
        switch(status){
            case "Proposed": return bulkUpload?.proposedFileCount ?? 0;
            case "DoNotUpload": return bulkUpload?.doNotUploadFileCount ?? 0;
            case "Pending": return bulkUpload?.pendingFileCount ?? 0;
            case "Success": return bulkUpload?.successFileCount ?? 0;
            case "Failed": return bulkUpload?.failedFileCount ?? 0;
            default: return 0;
        }
    }, [bulkUpload, status])

    return (
        <>
            <BulkUploadSingleHeader bulkUpload={bulkUpload} refresh={()=>{refresh(); refreshStatus(); }}/>

            <div style={{display: 'flex', gap: '1em', alignItems: 'center'}}>
                <Button disabled={!selected.length} onClick={()=>restartFiles()} variant={'contained'} color={'success'}>Upload selected files</Button>
                <h3>{status}</h3>
            </div>
            <div>
                <DataGrid
                    density={'compact'}
                    rowCount={rowCount}
                    paginationMode={'server'}
                    paginationModel={paginationModel}
                    onPaginationModelChange={setPaginationModel}
                    columns={columns}
                    rows={rows}
                    loading={isLoading || uploadIsLoading}
                    disableRowSelectionOnClick={true}
                    checkboxSelection={true}
                    rowSelectionModel={selected}
                    onRowSelectionModelChange={rowSelectionModel=>setSelected(rowSelectionModel as number[])}

                />
            </div>
        </>
    )
}