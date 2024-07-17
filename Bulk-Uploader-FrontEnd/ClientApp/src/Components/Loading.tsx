import React from "react";
import CircularProgress from "@mui/material/CircularProgress";
import "../App.css";
import { observer } from "mobx-react";

export interface LoadingProps {
    size?: number;
}
const Loading = observer((props: LoadingProps) => {
    return (
        <>
            <CircularProgress {...props} />
        </>
    );
});

export default Loading;
