import axios from "axios";
import { UAParser } from "ua-parser-js";
import type { UserInfo } from "../types/auth/types";

export const api = axios.create({
    baseURL: import.meta.env.VITE_API_AUTH_URL,
    timeout: 60000,
    headers: { "Content-Type": "application/json" },
});

export function getUserInfo(): UserInfo {
    const parser = new UAParser();
    const ua = parser.getResult();

    return {
        browser: `${ua.browser.name} ${ua.browser.version}`,
        os: `${ua.os.name} ${ua.os.version}`,
        device: ua.device.type ?? "desktop",
        locale: navigator.language,
        timeZone: Intl.DateTimeFormat().resolvedOptions().timeZone,
    };
}
