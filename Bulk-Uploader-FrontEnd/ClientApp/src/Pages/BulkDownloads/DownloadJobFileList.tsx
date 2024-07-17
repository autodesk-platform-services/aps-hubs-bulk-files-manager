import {DataGrid, GridColDef} from "@mui/x-data-grid";
import {IBulkDownload, IBulkDownloadFile} from "../../Utilities/Hooks/useBulkDownloads";
import {bytesToSize} from "./DownloadSingleStatus";

const columns: GridColDef<IBulkDownloadFile>[] = [
    {field: 'fileName', headerName: 'File Name', flex:1},
    {field: 'sourceFilePath', headerName: 'Source File Path', flex:1},
    {field: 'destinationFilePath', headerName: 'Download Path', flex:1},
    {field: 'fileSize', headerName: 'File Size', sortable: false, width:100, type: 'number',
        renderCell: ({row})=>bytesToSize(row.fileSize)},
    {field: 'status', headerName: 'Status', width:80},
]

export const DownloadJobFileList = ({download}: {download: IBulkDownload}) => {
    return (
        <DataGrid columns={columns} rows={download?.files ?? []} />
    )
}