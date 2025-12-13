import type { LoginForm } from "../../types/auth/types";
import { api } from "../../utils/utils";
import type { ISendCodeResult, IVerifyCodeResponse } from "./types";

export async function sendCodeAsync(login: string): Promise<ISendCodeResult> {
    const res = await api.post<ISendCodeResult>("/sendCode", {
        login: login,
    });

    return res.data;
}

export async function verifiCodeAsync(
    data: LoginForm
): Promise<IVerifyCodeResponse> {
    const res = await api.post<IVerifyCodeResponse>("/verify", {
        login: data.login,
        code: data.code.join(""),
        userInfo: data.userInfo,
    });

    return res.data;
}
