import {useHubs, useProjects} from "../Utilities/Hooks/useDataManagement";
import Button from "@mui/material/Button"
import {Autocomplete, DialogActions, DialogContent, DialogTitle, TextField} from "@mui/material";
import {useMemo, useState} from "react";
import FolderSelector from "./FolderSelector";
import Box from "@mui/material/Box";
import Typography from "@mui/material/Typography";

export interface ICloudFolderProps{
    hubId: string;
    projectId: string;
    folderId: string;
    folderPath?: string;
}
export interface ProjectTreeProps {
    type: 'upload' | 'download';
    onChange: ({hubId, projectId, folderId, folderPath}: ICloudFolderProps) => void;
    onSubmit: (open: boolean) => void;
}
export const ProjectTree = (props: ProjectTreeProps) => {
    const [selectedHub, setSelectedHub] = useState<string>();
    const [selectedProject, setSelectedProject] = useState<string>();
    const [folderId, setFolderId] = useState<string>();
    const [folderPath, setFolderPath] = useState<string>();
    
    
    const {isFetching: isHubsLoading, data: hubs, } = useHubs({refetchOnMount: true});

    const {
        isFetching: isProjectsLoading,
        data: projects,
        } = useProjects(selectedHub!, {enabled: selectedHub != null});

    const onFolderChange = (folderId: string, folderPath: string) => {
        setFolderId(folderId);
        setFolderPath(folderPath);
        if(selectedProject && selectedHub && folderId)
        props.onChange({
            hubId: selectedHub,
            projectId: selectedProject,
            folderId,
            folderPath
        });
    };
    
    const onSubmit = (open: boolean) => {
        props.onSubmit(false)
    }

    const title = useMemo(()=>{
        switch(props.type){
            case 'upload': return "Select an upload destination";
            case 'download': return "Select a download destination";
            default: return "Select a destination";
        }
    }, [props.type])

    return(
        <>
            <DialogTitle>{title}</DialogTitle>
            <DialogContent sx={{display: 'flex', flexDirection: 'column', gap: '.75em'}}>
                <Autocomplete 
                    sx={{m:1}}
                    options={hubs ?? []}
                    disabled={isHubsLoading}
                    onChange={(event, newValue) => {
                            setSelectedHub(newValue?.id)
                        }
                    }
                    getOptionDisabled={option=>(option.id.startsWith('a.') && props.type === 'upload')}
                    getOptionLabel={(option) => option.name}
                    renderInput={(params) => <TextField {...params} label="Select an account"/>}
                    />

                <Autocomplete
                    sx={{m:1}}
                    options={projects ?? []}
                    disabled={selectedHub == null && !isProjectsLoading}
                    onChange={(event, newValue) => {
                        setSelectedProject(newValue?.id)
                    }}
                    getOptionLabel={(option) => option.name}
                    renderInput={(params) => <TextField {...params} label="Select a project"/>}
                />

                <Box>
                    {selectedHub && selectedProject && !isProjectsLoading ? (
                        <>
                            <Box sx={{ display: "flex", alignItems: "center" }}>
                                <Typography m={1} variant="body2" sx={{ flex: 1 }}>
                                    Choose a project folder:
                                </Typography>
                            </Box>
                            <FolderSelector
                                hub={hubs?.find(x => x.id === selectedHub)}
                                project={projects?.find(x=> x.id === selectedProject)}
                                onChange={onFolderChange}
                            />
                        </>
                    ):
                    <></>}
                </Box>
                <DialogActions>

                    <Button
                        fullWidth={true}
                        onClick={()=>onSubmit(true)}
                    >Cancel</Button>
                    <Button
                        fullWidth={true}
                        variant={"contained"}
                        onClick={()=>onSubmit(true)}
                    >Finish</Button>
                </DialogActions>
            </DialogContent>
        </>
    )
}