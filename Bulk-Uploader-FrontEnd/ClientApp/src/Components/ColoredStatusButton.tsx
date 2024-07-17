import Button from "@mui/material/Button";
import { NavLink } from "react-router-dom";


export function ColoredStatusButton({ color, count, path, title }: {
    color: "inherit" | "primary" | "secondary" | "success" | "error" | "info" | "warning",
    count?: number,
    path: string,
    title?: string
}) {
    return (
        <Button
            title={title}
            component={NavLink}
            to={path}
            color={color}
            variant={count ? 'contained' : 'outlined'}
            onClick={(e) => e.stopPropagation()}
        >
            {count}
        </Button>
    )
}

