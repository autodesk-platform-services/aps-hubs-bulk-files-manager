import {
    Autocomplete,
    Dialog, DialogActions, DialogContent,
    DialogTitle,
    FormControl,
    InputLabel,
    OutlinedInput,
    Paper, Switch,
    TextField
} from "@mui/material";
import React, {useState} from "react";
import Button from "@mui/material/Button";
import IconButton from "@mui/material/IconButton";
import FolderIcon from '@mui/icons-material/Folder';
import {useMessageListener} from "../../Utilities/Hooks/useMessageListener.tsx";
import {IBulkUploadFile, useBulkUploads} from "../../Utilities/Hooks/useBulkUploads.ts";
import {useNavigate} from "react-router";
import {useScripting} from "../../Utilities/Hooks/useScripting.ts";
import {IBulkUploadPreset, useBulkUploadPresets} from "../../Utilities/Hooks/useBulkUploadPresets.ts";
import {ICloudFolderProps} from "../../Components/projectTree";
import Typography from "@mui/material/Typography";
import {AuthenticationButton} from "../../Components/AuthenticationButton.tsx";
import {BrowseCloudLocation} from "../../Components/BrowseCloudLocation.tsx";

export const BulkUploadCreate = () => {
    const navigate = useNavigate();
    const {mutation} = useBulkUploads();

    const {data: presets} = useBulkUploadPresets({refetchOnMount: true});
    const [showAdvanced, setShowAdvanced] = useState(false);
    const [name, setName] = useState(new Date().toLocaleString());
    const [localPath, setLocalPath] = useState("");
    const [projectId, setProjectId] = useState("");
    const [folderId, setFolderId] = useState("");
    const [folderUrlMessage, setFolderUrlMessage] = useState("");

    const [excludedFileTypes, setExcludedFileTypes] = useState("");
    const [excludedFolderNames, setExcludedFolderNames] = useState("");
    const [modifyPathScript, setModifyPathScript] = useState(
        `/* Available Variables:
 fileName,  relativeFilePath,  rootFolderPath;
 */
    \r\nreturn {
        "shouldUpload": true,
        "targetFileName": fileName,
        "targetRelativePath": relativeFilePath
    }`);
    const [useModifyPathScript, setUseModifyPathScript] = useState(false);
    const [testSourceAbsolutePath, setTestSourceAbsolutePath] = useState("");
    const [result, setResult] = useState<IBulkUploadFile>();
    const [resultError, setResultError] = useState(false);
    const [showSavePresetDialog, setShowSavePresetDialog] = useState(false);
    const [showDeletePresetDialog, setShowDeletePresetDialog] = useState(false);
    const [selectedPreset, setSelectedPreset] = useState<IBulkUploadPreset | null>(null);

    const {mutation: mutateScripting} = useScripting();

    function testScripting() {
        if (!testSourceAbsolutePath) {
            setResultError(true);
            setResult(undefined);
            return;
        }

        mutateScripting.mutateAsync({
            command: 'post',
            bulkUpload: {
                localPath: testSourceAbsolutePath.slice(0, testSourceAbsolutePath.indexOf("\\")),
                modifyPathScript,
                useModifyPathScript,
                excludedFileTypes,
                excludedFolderNames
            },
            bulkUploadFile: {
                sourceFileName: testSourceAbsolutePath.split('\\').pop(),
                sourceAbsolutePath: testSourceAbsolutePath,
                sourceRelativePath: testSourceAbsolutePath.slice(0, testSourceAbsolutePath.lastIndexOf('\\'))
            }
        })
            .then((bulkUploadFile) => {
                setResult(bulkUploadFile);
                setResultError(!bulkUploadFile);
            })
            .catch((response) => {
                setResultError(true);
                setResult(response);
            })
    }

    function submitRun(status: "Preview" | "Pending") {
        mutation.mutateAsync({
            command: 'post', bulkUploads: [{
                name,
                localPath,
                projectId,
                folderId,
                excludedFolderNames,
                excludedFileTypes,
                modifyPathScript,
                useModifyPathScript,
                status
            }]
        })
            .then(() => {
                navigate(`/bulkUploads`)
            })
    }

    const onFolderChange = ({hubId, projectId, folderId, folderPath}: ICloudFolderProps) => {
        setFolderId(folderId!);
        setProjectId(projectId);
        setFolderUrlMessage(`Destination: ${folderPath}`);
    };

    function selectPreset(preset?: IBulkUploadPreset) {
        if (!preset) return;

        setExcludedFileTypes(preset.excludedFileTypes)
        setModifyPathScript(preset.modifyPathScript)
        setExcludedFolderNames(preset.excludedFolderNames)
        setUseModifyPathScript(preset.useModifyPathScript)
        setSelectedPreset(preset)
    }

    const isValid = name && localPath && projectId && folderId;

    return <form style={{display: 'flex', flexDirection: 'column', gap: '.5em'}}>

        <div style={{display: 'flex', justifyContent: 'space-between', alignItems: 'center'}}>
            <h2>Create Bulk Upload</h2>
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
            <LocalFolderPathTextArea
                label={"Local Folder Path"}
                value={localPath}
                setValue={setLocalPath}
                message={'getFolderPath'}
            />

            <h3>Destination</h3>
            <BrowseCloudLocation onFolderChange={onFolderChange} type={'upload'}/>

        </div>


        <Typography mt={1}>{folderUrlMessage}</Typography>

        <div>
            <Button onClick={() => setShowAdvanced(!showAdvanced)}>Advanced Filters</Button>
        </div>
        <Paper style={{
            backgroundColor: '#efefef',
            padding: '1em',
            display: showAdvanced ? 'flex' : 'none',
            flexDirection: 'column',
            gap: '1em'
        }}>

            <div style={{display: 'flex', gap: '1em'}}>

                <Autocomplete
                    style={{backgroundColor: 'white', flex: 1}}
                    renderInput={params => (<TextField {...params} label={"Presets"}/>)}
                    options={presets ?? []}
                    getOptionLabel={preset => preset.name}
                    onChange={(event, preset) => {
                        selectPreset(preset || undefined);
                    }}
                />

                <Button onClick={() => setShowSavePresetDialog(true)}>Save as New Preset</Button>
                {showSavePresetDialog && <SavePresetDialog
                    handleClose={() => setShowSavePresetDialog(false)}
                    excludedFileTypes={excludedFileTypes}
                    excludedFolderNames={excludedFolderNames}
                    modifyPathScript={modifyPathScript}
                    useModifyPathScript={useModifyPathScript}
                />}

                <Button onClick={() => setShowDeletePresetDialog(true)}>Delete Preset</Button>
                {showDeletePresetDialog && <DeletePresetDialog
                    handleClose={() => {
                        setShowDeletePresetDialog(false)
                        setSelectedPreset(null)
                    }
                    }
                    id={selectedPreset?.id}
                />}
            </div>

            <TextField
                inputProps={{style: {backgroundColor: 'white'}}}
                label={"File Types to Exclude (Comma separated values)"}
                value={excludedFileTypes}
                onChange={(e) => setExcludedFileTypes(e.currentTarget.value ?? "")}
            />
            <TextField
                inputProps={{style: {backgroundColor: 'white'}}}
                label={"Folder Names to Exclude (Comma separated values)"}
                value={excludedFolderNames}
                onChange={(e) => setExcludedFolderNames(e.currentTarget.value ?? "")}
            />

            <Paper style={{display: 'flex', flexDirection: 'column', gap: '1em', padding: '1em'}}>

                <div style={{display: 'flex', gap: '1em', alignItems: 'center'}}>
                    <Switch id={'use-modify-path-script'} checked={useModifyPathScript}
                            onChange={(e, checked) => setUseModifyPathScript(checked)}/>
                    <InputLabel htmlFor={'use-modify-path-script'}>Use Javascript Filter</InputLabel>
                </div>
                <div style={{
                    display: useModifyPathScript ? 'flex' : 'none',
                    flexDirection: 'column',
                    gap: '1em',
                    padding: '1em'
                }}>
                    <div>
                        <p>
                            This filter allows you to use Javascript to determine if a file should be uploaded, as well
                            as
                            the
                            path it should be uploaded to. The code should set a global variable
                            called <code>outputValue</code> that is
                            set to either the relative file path, or <code>false</code> if it should not be uploaded.
                            The
                            following
                            variables are available for your use:
                        </p>

                        <ul>
                            <li>rootFolderPath: This is equivalent to the "Local Folder Path" value
                                (<code>C:/users/me/Downloads</code>)
                            </li>
                            <li>absoluteFilePath: This is equivalent to the current file path
                                (<code>C:/users/me/Downloads/MyTargetFolder/ChildFolder/myFile.pdf</code>)
                            </li>
                            <li>relativeFilePath: This is equivalent to the current file path
                                (<code>/MyTargetFolder/ChildFolder</code>)
                            </li>
                            <li>fileName: The current file name (<code>myfile.pdf</code>)</li>
                            <li>cloudFolders: An array of relative folder paths and folder URNs in ACC / BIM 360
                                (<code>{`[{
                                folderName: "myFolder", 
                                relativeFolderPath: "/", 
                                folderUrl: "https://acc.autodesk.com[...]", 
                                folderUrn: "urn:folder:2q3af"
                                }]`}</code></li>
                        </ul>
                    </div>

                    <TextField
                        inputProps={{
                            spellCheck: 'false',
                            style: {
                                fontFamily: 'monospace'
                            }
                        }}
                        autoFocus={true}
                        rows={15}
                        multiline
                        label={"Javascript Filter"}
                        value={modifyPathScript}
                        onChange={(e) => setModifyPathScript(e.currentTarget.value ?? "")}
                        error={resultError}
                    />
                </div>
            </Paper>
            <div style={{display: 'flex', gap: '1em'}}>
                <LocalFolderPathTextArea
                    label={"Input Value (File Path)"}
                    value={testSourceAbsolutePath}
                    setValue={setTestSourceAbsolutePath}
                    message={'getFilePath'}
                />
                <Button onClick={testScripting}>Test Filters</Button>
            </div>
            {!!result && <code>
                <div style={{display: 'grid', gridTemplateColumns: 'auto 1fr', gap: '1em'}}>
                    <div>STATUS:</div>
                    <div>{result.status}</div>

                    <div>NEW PATH:</div>
                    <div>{result.targetRelativePath}</div>

                    <div>NEW FILE NAME:</div>
                    <div>{result.targetFileName}</div>
                </div>
                <br/>
                {JSON.stringify(result, null, 2)}
            </code>}


        </Paper>
        <br/>
        <div style={{display: 'flex', gap: '1em', alignItems: 'center', margin: '0 20vw'}}>
            <Button onClick={() => submitRun("Preview")} style={{flex: 1}} disabled={!isValid} color={'secondary'}
                    variant={'contained'}>Create Dry Run</Button>
            <Button onClick={() => submitRun("Pending")} style={{flex: 1}} disabled={!isValid} color={'success'}
                    variant={'contained'}>Upload Files</Button>
        </div>
    </form>
}

export const DeletePresetDialog = ({handleClose, id}: { handleClose: () => void, id: number | undefined }) => {
    const {presetMutation} = useBulkUploadPresets({});
    if (!id) {
        handleClose()
    }

    function deletePreset() {
        presetMutation.mutateAsync({
            command: 'delete', presets: [
                {
                    id
                }
            ]
        })
            .finally(handleClose)
    }

    return (
        <Dialog open={true} onClose={handleClose}>
            <DialogTitle>Delete Preset?</DialogTitle>
            <DialogActions>
                <Button onClick={handleClose}>Cancel</Button>
                <Button onClick={deletePreset}>Confirm</Button>
            </DialogActions>
        </Dialog>
    )
}

export const SavePresetDialog = ({
                                     handleClose,
                                     excludedFileTypes,
                                     excludedFolderNames,
                                     useModifyPathScript,
                                     modifyPathScript
                                 }: {
    excludedFileTypes: string,
    excludedFolderNames: string,
    useModifyPathScript: boolean,
    modifyPathScript: string,
    handleClose: () => void
}) => {
    const [name, setName] = useState("")
    const {presetMutation} = useBulkUploadPresets({});


    function saveChanges() {
        presetMutation.mutateAsync({
            command: 'post', presets: [
                {
                    name,
                    excludedFileTypes,
                    excludedFolderNames,
                    modifyPathScript,
                    useModifyPathScript
                }
            ]
        })
            .finally(handleClose)
    }

    return (
        <Dialog open={true} onClose={handleClose}>
            <DialogTitle>Save as new Preset</DialogTitle>
            <DialogContent style={{padding: '1em'}}>
                <TextField
                    label={"Preset Name"}
                    value={name}
                    onChange={(e) => setName(e.currentTarget.value ?? "")}
                    autoFocus={true}
                />
            </DialogContent>
            <DialogActions>
                <Button onClick={handleClose}>Cancel</Button>
                <Button onClick={saveChanges}>Create new Preset</Button>
            </DialogActions>

        </Dialog>
    )
}


export const LocalFolderPathTextArea = ({label, value, setValue, message}: {
    label: string,
    value: string,
    setValue: (key: string) => void,
    message: 'getFolderPath' | 'getFilePath'
}) => {
    const id = btoa(label);

    const {sendMessage} = useMessageListener(message, setValue)

    function getPath() {
        sendMessage(message);
    }

    return (
        <FormControl variant="outlined" style={{flex: 1}}>
            <InputLabel htmlFor={id}>{label}</InputLabel>
            <OutlinedInput
                id={id}
                value={value}
                sx={{backgroundColor: 'white'}}
                onChange={(e) => setValue(e.currentTarget.value ?? "")}
                endAdornment={<IconButton onClick={getPath}>
                    <FolderIcon/>
                </IconButton>}
            />
        </FormControl>
    )
}