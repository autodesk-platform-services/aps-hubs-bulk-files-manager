import {useEffect, useState} from "react";

export function useMessageListener(type: string, callback: (key: string)=>void) {
    const [data, setData] = useState("")

    useEffect(() => {
        if (window) {
// eslint-disable-next-line @typescript-eslint/ban-ts-comment
//@ts-ignore
            window.chrome.webview.addEventListener('message', (event) => {
                try {
                    const parsedData =event.data as {type: string, data: string};
                    if (parsedData.type === type) {
                        setData(parsedData.data)
                        if(callback) callback(parsedData.data);
                    }
                } catch (e) {
                    console.error(e);
                }
            })
        }

// eslint-disable-next-line @typescript-eslint/ban-ts-comment
//@ts-ignore
        return window.chrome.webview.removeEventListener('message', ()=>{})
    }, [])

    function sendMessage(message: string) {
// eslint-disable-next-line @typescript-eslint/ban-ts-comment
        //@ts-ignore
        window.chrome.webview.postMessage(message);
    }

    return {data, sendMessage}
}