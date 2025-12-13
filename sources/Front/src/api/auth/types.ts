export interface ISendCodeResult {
    isSuccess: boolean;
    loginType: LoginType;
}

export type LoginType = "email" | "phone";

export interface IVerifyCodeRequest {
    code: string;
    login: string;
}

export interface IVerifyCodeResponse {
    isSuccess: boolean;
    code: string;
}
