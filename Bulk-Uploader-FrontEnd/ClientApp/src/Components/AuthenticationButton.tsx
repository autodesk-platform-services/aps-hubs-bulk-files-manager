import {Button, ButtonGroup} from "@mui/material";
import {useIsAuthenticated, useThreeLegged} from "../Utilities/Hooks/useThreeLegged.ts";
import IconButton from "@mui/material/IconButton";
import {Sync} from "@mui/icons-material";
import {useNavigate} from "react-router";
import React from "react";
import Box from "@mui/material/Box";



export function AuthenticationButton() {
    const {isAuthenticated} = useIsAuthenticated();
    const {authUrl, logout} = useThreeLegged();
    const navigate = useNavigate();

    function login(e: React.SyntheticEvent) {
        e.preventDefault();

        // http://stackoverflow.com/questions/4068373/center-a-popup-window-on-screen
        // Fixes dual-screen position  Most browsers      Firefox
        const w = 600;
        const h = 800;

        const dualScreenLeft = window.screenLeft != undefined ? window.screenLeft : 0;
        const dualScreenTop = window.screenTop != undefined ? window.screenTop : 0;

        const width = window.innerWidth ? window.innerWidth : document.documentElement.clientWidth ? document.documentElement.clientWidth : screen.width;
        const height = window.innerHeight ? window.innerHeight : document.documentElement.clientHeight ? document.documentElement.clientHeight : screen.height;

        const left = ((width / 2) - (w / 2)) + dualScreenLeft;
        const top = ((height / 2) - (h / 2)) + dualScreenTop;
        const newWindow = window.open(authUrl, '_blank', 'scrollbars=yes, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);

        newWindow?.focus();
    }

    return (<Box sx={{display: "flex", gap:1}}>
        <ButtonGroup>
            <Button
                color={'secondary'}
                onClick={()=>logout.mutate()}
                variant={isAuthenticated ? 'outlined' : 'contained'}
            >App (2-Legged)</Button>
            <a href={authUrl} target={"_blank"} rel="noopener noreferrer">
            <Button
                color={'warning'}
                onClick={login}
                variant={isAuthenticated ? 'contained' : 'outlined'}
            >User (3-Legged)</Button></a>   
        </ButtonGroup>
        <IconButton>
            <Sync onClick={()=>window.location.reload()}/>
        </IconButton>
    </Box>)
}