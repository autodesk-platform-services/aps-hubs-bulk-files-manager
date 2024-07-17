import {Outlet} from "react-router";
import ResponsiveAppBar from "./ResponsiveAppBar.tsx";

export const Layout = ()=>{
    return <>
        <ResponsiveAppBar/>

        <main>
            <Outlet/>
        </main>
    </>
}