import axios, {AxiosRequestConfig} from "axios";

export function SimplePost<T>(url: string, body?: any, options?: any): Promise<T> {
    return new Promise((resolve, reject) => {
        axios.post(url, body, options)
            .then(result => {
                resolve(result?.data)
            })
            .catch(error => {
                if(error.response?.status !== 401){

                }
                reject(error);
            })
    })
}

export function SimplePut<T>(url: string, body?: any, options?: any): Promise<T> {
    return new Promise((resolve, reject) => {
        axios.put(url, body, options)
            .then(result => {
                resolve(result?.data)
            })
            .catch(error => {
                if(error.response?.status !== 401){

                }
                reject(error);
            })
    })
}

export function SimplePatch<T>(url: string, body?: any, options?: any): Promise<T> {
    return new Promise((resolve, reject) => {
        axios.patch(url, body, options)
            .then(result => {
                resolve(result?.data)
            })
            .catch(error => {
                console.error(error)
                if(error.response?.status !== 401){

                }
                reject(error);
            })
    })
}

export function SimpleDelete<T>(url: string, options?: any): Promise<T> {
    return new Promise((resolve, reject) => {
        axios.delete(url, options)
            .then(result => {
                resolve(result?.data)
            })
            .catch(error => {

                reject(error);
            })
    })
}

export function  SimpleGet<T>(url: string, options?: AxiosRequestConfig): Promise<T>{
    return new Promise((resolve, reject) => {
        axios.get(url, options)
            .then(result => {
                resolve(result?.data as T)
            })
            .catch(error => {
                if(error.response?.status !== 401){

                }
                reject(error);
            })
    })
}

export function  SimpleDownload<T>(url: string, options?: any): Promise<T>{
    return new Promise((resolve, reject) => {
        axios.get(url, {responseType: 'blob', ...options})
            .then(result => {
                resolve(result?.data)
            })
            .catch(error => {
                if(error.response?.status !== 401){

                }
                reject(error);
            })
    })
}