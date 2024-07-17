import {Button, Paper, TextField} from "@mui/material";
import {useRef, useState} from "react";
import {ISetting, useSettings} from "../Utilities/Hooks/useSettings.ts";
import Box from "@mui/material/Box";
import {Settings} from "@mui/icons-material";
import {IBulkUploadPreset, useBulkUploadPresets} from "../Utilities/Hooks/useBulkUploadPresets";
import {useThreeLegged} from "../Utilities/Hooks/useThreeLegged.ts";

export const InitialSetupPage = ()=>{
    const {refresh, authUrl} = useThreeLegged();
    const {dataHash: settingsHash, mutation} = useSettings();
    const {presetMutation} = useBulkUploadPresets({});

    const [clientId, setClientId] = useState<string>(settingsHash["ClientId"]?.value ?? "");
    const [clientSecret, setClientSecret] = useState<string>(settingsHash["ClientSecret"]?.value ?? "");
    const handleFileSelect = (event: any) => {
        
        const file = event.target.files[0];

        if(file){
            const fr = new FileReader();
            fr.onload = function(e) {
                var content = e.target?.result as string;
                var {Settings, presets} = JSON.parse(content) as {Settings: {[key: string]: ISetting}, presets: IBulkUploadPreset[]};
                
                const settings = Object.values(Settings) as ISetting[];
                mutation.mutateAsync({command: 'patch', settings: settings })
                presetMutation.mutateAsync({command: 'post', presets})
            }
                fr.readAsText(file);
        }
    };

    
    
    function saveChanges(){
        if(clientId && clientSecret){
            const clientIdSetting = settingsHash["ClientId"] ?? {key: 'ClientId', value: ''}
            const clientSecretSetting = settingsHash["ClientSecret"] ?? {key: 'ClientSecret', value: ''}

            clientIdSetting.value = clientId;
            clientSecretSetting.value = clientSecret;

            mutation.mutateAsync({command: 'patch', settings: [clientIdSetting, clientSecretSetting]})
                .then(()=>refresh())
        }
    }

    const isSaveDisabled = (!clientId || !clientSecret);

    return (<div style={{padding: '2em'}}>
    <h1>Autodesk Bulk File Manager</h1>

        <Paper style={{padding: '1em', display: 'flex', flexDirection: 'column', gap: '1em'}}>

            <TextField
                label={"Autodesk Client ID"}
                value={clientId}
                onChange={(e)=>setClientId(e.currentTarget.value ?? "")}
            />
            <TextField
                label={"Autodesk Client Secret"}
                value={clientSecret}
                type={'password'}
                onChange={(e)=>setClientSecret(e.currentTarget.value ?? "")}
            />
            
        </Paper>
        <div style={{width: '100%'}}>
            <Box
                sx={{
                    display:'flex',
                    flexDirection: 'row-reverse',
                    pt:1,
                    
                }}>
                <Button
                    variant={isSaveDisabled ? undefined : 'contained'}
                    disabled={isSaveDisabled}
                    onClick={saveChanges}
                    sx={{width:'50%'}}
                >
                    Save & Log In
                </Button>

                <Button sx={{m:1, width:'50%'}}
                        variant={"outlined"}
                        component="label"
                >
                    Import Settings
                    <input
                        type="file"
                        accept="application/json"
                        multiple={false}
                        onChange={(e) => handleFileSelect(e)}
                        hidden
                    />
                </Button>
            </Box>
        </div>

    </div>)
}