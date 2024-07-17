import * as React from 'react';
import AppBar from '@mui/material/AppBar';
import Box from '@mui/material/Box';
import Toolbar from '@mui/material/Toolbar';
import IconButton from '@mui/material/IconButton';
import Typography from '@mui/material/Typography';
import Menu from '@mui/material/Menu';
import MenuIcon from '@mui/icons-material/Menu';
import Container from '@mui/material/Container';
import Button from '@mui/material/Button';
import Tooltip from '@mui/material/Tooltip';
import MenuItem from '@mui/material/MenuItem';
import {AutodeskIcon} from "../Images/AutodeskIcon.tsx";
import {NavLink} from "react-router-dom";
import SettingsIcon from '@mui/icons-material/Settings';

interface IPage{
    label: string;
    path: string;
    reloadDocument?: boolean;
    disabled?: boolean;
}

const pages: IPage[] = [
    {label: 'Create Upload', path: '/bulkUploads/create', reloadDocument: false},
    {label: 'Upload History', path: '/bulkUploads', reloadDocument: false},
    {label: 'Create Download', path: '/downloads/create', reloadDocument: false},
    {label: 'Download History', path: '/downloads', reloadDocument: false},
    // {label: 'Utilities', path: '/utilities', reloadDocument: false},
    // {label: 'Batches', path: '/batches', reloadDocument: false},
    // {label: 'Bulk Operations', path: '/selections', reloadDocument: false, disabled: false},
];
const settings: IPage[] = [
    {label: 'Hangfire', path: '/hangfire', reloadDocument: true},
    {label: 'Settings', path: '/settings', reloadDocument: false},
    {label: 'Utilities', path: '/utilities', reloadDocument: false, disabled: false}
];

function ResponsiveAppBar() {
    const [anchorElNav, setAnchorElNav] = React.useState<null | HTMLElement>(null);
    const [anchorElUser, setAnchorElUser] = React.useState<null | HTMLElement>(null);

    const handleOpenNavMenu = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorElNav(event.currentTarget);
    };
    const handleOpenUserMenu = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorElUser(event.currentTarget);
    };

    const handleCloseNavMenu = () => {
        setAnchorElNav(null);
    };

    const handleCloseUserMenu = () => {
        setAnchorElUser(null);
    };

    return (
        <AppBar position="static">
            <Container maxWidth={false}>
                <Toolbar disableGutters sx={{gap: '1em'}}>
                    <AutodeskIcon sx={{display: {xs: 'none', md: 'flex'}, mr: 1, fill: '#fff'}}/>
                    <Typography
                        variant="h6"
                        noWrap
                        component="a"
                        href="/"
                        sx={{
                            mr: 2,
                            display: {xs: 'none', md: 'flex'},
                            fontFamily: 'monospace',
                            fontWeight: 700,
                            letterSpacing: '.15rem',
                            color: 'inherit',
                            textDecoration: 'none',
                        }}
                    >
                        Bulk File Manager
                    </Typography>

                    <Box sx={{flexGrow: 1, display: {xs: 'flex', md: 'none'}}}>
                        <IconButton
                            size="large"
                            aria-label="account of current user"
                            aria-controls="menu-appbar"
                            aria-haspopup="true"
                            onClick={handleOpenNavMenu}
                            color="inherit"
                        >
                            <MenuIcon/>
                        </IconButton>
                        <Menu
                            id="menu-appbar"
                            anchorEl={anchorElNav}
                            anchorOrigin={{
                                vertical: 'bottom',
                                horizontal: 'left',
                            }}
                            keepMounted
                            transformOrigin={{
                                vertical: 'top',
                                horizontal: 'left',
                            }}
                            open={Boolean(anchorElNav)}
                            onClose={handleCloseNavMenu}
                            sx={{
                                display: {xs: 'block', md: 'none'},
                            }}
                        >
                            {pages.map((page) => (
                                <MenuItem disabled={page.disabled} key={page.label} component={NavLink} reloadDocument={page.reloadDocument}
                                          to={page.path} onClick={handleCloseNavMenu}>
                                    <Typography textAlign="center">{page.label}</Typography>
                                </MenuItem>
                            ))}
                        </Menu>
                    </Box>
                    <AutodeskIcon sx={{display: {xs: 'flex', md: 'none'}, mr: 1, fill: '#fff'}}/>
                    <Typography
                        variant="h5"
                        noWrap
                        component="a"
                        href=""
                        sx={{
                            mr: 2,
                            display: {xs: 'flex', md: 'none'},
                            flexGrow: 1,
                            fontFamily: 'monospace',
                            fontWeight: 700,
                            letterSpacing: '.3rem',
                            color: 'inherit',
                            textDecoration: 'none',
                        }}
                    >
                        Bulk File Manager
                    </Typography>
                    <Box sx={{flexGrow: 1, display: {xs: 'none', md: 'flex'}}}>
                        {pages.map((page) => (

                            <Button
                                disabled={page.disabled}
                                component={NavLink}
                                reloadDocument={page.reloadDocument}
                                to={page.path}
                                key={page.label}
                                onClick={handleCloseNavMenu}
                                sx={{my: 2, color: 'white', display: 'block'}}
                            >
                                {page.label}
                            </Button>
                        ))}
                    </Box>

                    <Box sx={{flexGrow: 0}}>
                        <Tooltip title="Open settings">
                            <IconButton onClick={handleOpenUserMenu} sx={{p: 0}}>
                                <SettingsIcon sx={{color: '#fff'}}/>
                            </IconButton>
                        </Tooltip>
                        <Menu
                            sx={{mt: '45px'}}
                            id="menu-appbar"
                            anchorEl={anchorElUser}
                            anchorOrigin={{
                                vertical: 'top',
                                horizontal: 'right',
                            }}
                            keepMounted
                            transformOrigin={{
                                vertical: 'top',
                                horizontal: 'right',
                            }}
                            open={Boolean(anchorElUser)}
                            onClose={handleCloseUserMenu}
                        >
                            {settings.map((setting) => (
                                <MenuItem
                                    disabled={setting.disabled}
                                    key={setting.label}
                                    component={NavLink}
                                    to={setting.path}
                                    onClick={handleCloseUserMenu}
                                    reloadDocument={setting.reloadDocument}
                                >

                                    <Typography textAlign="center">{setting.label}</Typography>
                                </MenuItem>
                            ))}
                        </Menu>
                    </Box>
                </Toolbar>
            </Container>
        </AppBar>
    );
}

export default ResponsiveAppBar;