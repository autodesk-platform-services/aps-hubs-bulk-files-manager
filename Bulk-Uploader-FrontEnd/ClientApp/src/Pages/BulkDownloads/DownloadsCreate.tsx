import {CircularProgress, Dialog, DialogContent, DialogTitle, Paper, TextField} from "@mui/material";
import React, {useState} from "react";
import {LocalFolderPathTextArea} from "./../BulkUploads/BulkUploadCreate";
import {ICloudFolderProps, ProjectTree} from "../../Components/projectTree";
import Button from "@mui/material/Button";
import Typography from "@mui/material/Typography";
import {useBulkDownloads, IBulkDownload} from '../../Utilities/Hooks/useBulkDownloads'
import {useNavigate} from "react-router";
import axios from "axios";
import {read, utils} from 'xlsx';
import {AuthenticationButton} from "../../Components/AuthenticationButton.tsx";
import {BrowseCloudLocation} from "../../Components/BrowseCloudLocation.tsx";

export const DownloadsCreate = () => {
    const navigate = useNavigate();
    const [name, setName] = useState(new Date().toLocaleString());
    const [localPath, setLocalPath] = useState("");
    const [hubId, setHubId] = useState("");
    const [projectId, setProjectId] = useState("");
    const [apsFolderId, setapsFolderId] = useState("");
    const [folderUrlMessage, setFolderUrlMessage] = useState("");
    const [cloudPath, setCloudPath] = useState("");
    const {mutation} = useBulkDownloads();
    const [showAdvanced, setShowAdvanced] = useState(false);
    const fileRef = React.useRef<any>(null);
    const [selectFile, setSelectFile] = useState();
    const [creatingBulk, setCreatingBulk] = useState(false);
    const [bulkMessage, setBulkMessage] = useState<string>("");

    const onFolderChange = ({hubId, projectId, folderId, folderPath}: ICloudFolderProps) => {
        setapsFolderId(folderId);
        setHubId(hubId);
        setProjectId(projectId);
        setFolderUrlMessage(`Source: ${folderPath}`);
    };
    const handleSelection = () => {
        if (fileRef != null && fileRef.current != null)
            fileRef.current.click();
    };
    const onSelectedFileChange = async (e: any) => {
        const file = e.target.files[0]
        setSelectFile(file);

        var reader = new FileReader();
        reader.onloadend = function (event) {
            var arrayBuffer = reader.result;
            // debugger

            const workbook = read(arrayBuffer)
            const json = utils.sheet_to_json(workbook.Sheets[workbook.SheetNames[0]])

            try {
                setCreatingBulk(true);
                axios.post(`/api/downloads/bulk?downloadFolder=${localPath}`, json)
                    .then(() => {
                        navigate('/downloads')
                    });

            } catch (error: any) {
                setBulkMessage(error.message)
                console.log(error)
            }
        };
        reader.readAsArrayBuffer(file);
    }

    async function GetAllAccountsAndProjects() {
        axios({
            url: '/api/utilities/all',
            method: "GET",
            responseType: "blob"
        })
            .then((response) => {
                // create file link in browser's memory
                const href = URL.createObjectURL(response.data);

                // create "a" HTML element with href to file & click
                const link = document.createElement("a");
                link.href = href;
                link.setAttribute("download", `BulkFileManager_AllAccountsAndProjects.xlsx`); //or any other extension
                document.body.appendChild(link);
                link.click();

                // clean up "a" element & remove ObjectURL
                document.body.removeChild(link);
                URL.revokeObjectURL(href);
            })
            .catch((e) => {
                console.error(e);
            })
    }

    async function submitRun(status: "Pending") {
        mutation.mutateAsync({
            command: 'post', bulkDownloads: [{
                name,
                cloudPath,
                localPath,
                projectId,
                hubId,
                apsFolderId,
                status
            }]
        })
            .then(() => {
                navigate('/downloads')
            })
    }

    const isValid = name && localPath && projectId && apsFolderId && hubId;

    return <form style={{display: 'flex', flexDirection: 'column', gap: '.5em'}}>

        <div style={{display: 'flex', justifyContent: 'space-between', alignItems: 'center'}}>
            <h2>Create Bulk Download</h2>
            <div>
                <AuthenticationButton/>
            </div>
        </div>


        <TextField
            label={"Run Name"}
            value={name}
            onChange={(e) => setName(e.currentTarget.value ?? "")}
        />

        <div style={{display: 'grid', gap: '1em', gridTemplateColumns: 'auto 1fr', alignItems: 'center'}}>
            <h3>Origin</h3>
            <BrowseCloudLocation type={'download'} onFolderChange={onFolderChange}/>
            <h3>Destination</h3>
            <LocalFolderPathTextArea
                label={"Local Folder Path"}
                value={localPath}
                setValue={setLocalPath}
                message={'getFolderPath'}
            />
        </div>


        <Typography mt={1}>{folderUrlMessage}</Typography>

        <div>
            <Button onClick={() => setShowAdvanced(!showAdvanced)}>Bulk Download</Button>
        </div>
        <Paper style={{
            backgroundColor: '#efefef',
            padding: '1em',
            display: showAdvanced ? 'flex' : 'none',
            flexDirection: 'column',
            gap: '1em'
        }}>
            <h3>Bulk Download Operations</h3>

            <div>
                <p>
                    The bulk download operations functionality is designed to create many download jobs simultaneously.
                    To use the bulk download operations functionality, click the button to get all hubs and projects.
                    Once the operation has finished and downloaded an XLSX file to your machine, open the file and
                    delete the Account / Project combinations you wish to <strong>NOT</strong> download files for. Once
                    complete, click the 'Create Bulk Download Jobs' button and upload the edited file.

                    A Local path must be provided above to enable the 'Create Bulk Download Jobs' button.
                </p>
            </div>

            <div style={{display: 'flex'}}>
                <Button
                    style={{width: "380px"}}
                    onClick={GetAllAccountsAndProjects}
                    variant="contained"
                >
                    Get All Hubs and Projects
                </Button>
                <Typography sx={{flex: 1}}/>
                <input
                    style={{display: 'none'}}
                    type="file"
                    ref={fileRef}
                    onChange={onSelectedFileChange}>
                </input>
                <Button
                    style={{width: "380px"}}
                    onClick={handleSelection}
                    variant="contained"
                    color={'secondary'}
                    disabled={!localPath}
                >
                    Create Bulk Download Jobs
                </Button>
            </div>

        </Paper>

        <div style={{display: 'flex', gap: '1em', alignItems: 'center', margin: '0 20vw'}}>
            <Button onClick={() => submitRun("Pending")} style={{flex: 1}} disabled={!isValid} color={'success'}
                    variant={'contained'}>Download Files</Button>
        </div>

        {creatingBulk && <BulkDialog handleClose={() => setCreatingBulk(false)} message={bulkMessage}/>}
    </form>
}
const BulkDialog = ({handleClose, message}: { message: string, handleClose: () => void }) => {
    return (
        <Dialog open={true}>
            <DialogTitle>Creating Bulk Upload</DialogTitle>
            <DialogContent>
                {message ? message : <CircularProgress/>}
                {message!! && <Button onClick={() => handleClose()}>Close</Button>}
            </DialogContent>
        </Dialog>
    )
}