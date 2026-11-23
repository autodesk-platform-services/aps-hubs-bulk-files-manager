import {SvgIcon, SvgIconProps} from "@mui/material";

export const BulkHaulIcon = (params: SvgIconProps) => {

    return (
        <SvgIcon {...params} viewBox="0 0 24 24">
            <path d="M12 2 L17 7 H14 V10 H10 V7 H7 Z"/>
            <rect x="4" y="12.5" width="16" height="2.5" rx="0.6" fillOpacity="0.6"/>
            <rect x="4" y="16" width="16" height="2.5" rx="0.6" fillOpacity="0.8"/>
            <rect x="4" y="19.5" width="16" height="2.5" rx="0.6"/>
        </SvgIcon>
    )
}