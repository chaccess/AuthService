export type LoginForm = {
    login: string;
    code: string[];
    userInfo: UserInfo | undefined;
};

export type UserInfo = {
    browser: string;
    os: string;
    device: string;
    locale: string;
    timeZone: string;
};
