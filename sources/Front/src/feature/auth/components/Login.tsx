import { AbsoluteCenter, Box, VStack } from "@chakra-ui/react";
import { useEffect, useState, type FC } from "react";
import { FormProvider, useForm } from "react-hook-form";
import FieldsetState from "./FieldsetState";
import { EnterCodeState } from "./EnterCodeState";
import type { LoginFormState } from "./types/AuthTypes";
import { sendCodeAsync } from "../../../api/auth";
import type { ISendCodeResult } from "../../../api/auth/types";
import type { LoginForm } from "../../../types/auth/types";
import { LoginFormDefaults } from "../../../types/auth/defaults";
import { useCountdown } from "./CountDown";
import { useNavigate, useSearchParams } from "react-router-dom";
import { getUserInfo } from "../../../utils/utils";

export const Login: FC = () => {
    const methods = useForm<LoginForm>({
        defaultValues: LoginFormDefaults,
        mode: "onChange",
    });
    const [formShowing, setFormShowing] = useState<LoginFormState>("form");
    const [sendCodeResult, setSendCodeResult] =
        useState<ISendCodeResult | null>(null);
    const [fadingFields, setFadingFields] = useState<boolean>(false);
    const [isLoadingFields, setIsLoadingFields] = useState<boolean>(false);
    const { secondsLeft, isRunning, start, reset } = useCountdown(60);
    const [verified, setVerified] = useState<boolean>(false);
    const [redirectURI, setRedirectURI] = useState<string>("");
    const [getTokenCode, setGetTokenCode] = useState<string>("");
    const fadeDuration = 300;

    const { handleSubmit, setValue } = methods;

    const redirect = useNavigate();
    const [params] = useSearchParams();

    useState(() => {
        setValue("userInfo", getUserInfo());

        const uri = params.get("redirectURI");
        if (uri && uri.length !== 0) {
            setRedirectURI(uri);
        }
    });

    const sendCode = async (data: LoginForm) => {
        if (isRunning) {
            setFadingFields(true);
            return;
        }
        setIsLoadingFields(true);
        try {
            const res = await sendCodeAsync(data.login);
            setSendCodeResult(res);
            reset();
            start();
        } catch {
        } finally {
            setIsLoadingFields(false);
        }
    };

    const animationCompleted = () => {
        if (formShowing == "form") {
            setFormShowing("code");
        } else {
            setFormShowing("form");
        }
    };

    const backToFieldsForm = () => {
        setFadingFields(false);
    };

    useEffect(() => {
        if (sendCodeResult) setFadingFields(true);
    }, [sendCodeResult]);

    function buildRedirectURI(url: string, code: string) {
        // относительные пути оставляем как есть
        if (url.startsWith("/")) return url;

        // если протокола нет — добавим https://
        if (!/^https?:\/\//i.test(url)) return `https://${url}?code=${code}`;

        return url;
    }

    useEffect(() => {
        if (verified && getTokenCode.length > 0) {
            window.location.replace(
                buildRedirectURI(redirectURI, getTokenCode)
            );
        }
    }, [verified, getTokenCode]);

    useEffect(() => {
        if (redirectURI.length === 0) redirect("/404");
    }, [redirectURI]);

    return (
        <>
            <AbsoluteCenter>
                <VStack>
                    <Box
                        rounded="sm"
                        borderWidth="1px"
                        padding={10}
                        minH={290}
                        maxW={362}
                    >
                        <VStack as={"form"} onSubmit={handleSubmit(sendCode)}>
                            <FormProvider {...methods}>
                                {formShowing === "form" ? (
                                    <FieldsetState
                                        startFading={fadingFields}
                                        fadeDuration={fadeDuration}
                                        animationCompleted={animationCompleted}
                                        isLoading={isLoadingFields}
                                    />
                                ) : (
                                    sendCodeResult && (
                                        <EnterCodeState
                                            startFading={!fadingFields}
                                            fadeDuration={fadeDuration}
                                            loginType={sendCodeResult.loginType}
                                            animationCompleted={
                                                animationCompleted
                                            }
                                            backToFieldsForm={backToFieldsForm}
                                            resendCode={handleSubmit(sendCode)}
                                            isRunning={isRunning}
                                            secondsLeft={secondsLeft}
                                            setVerified={setVerified}
                                            setGetTokenCode={setGetTokenCode}
                                        />
                                    )
                                )}
                            </FormProvider>
                        </VStack>
                    </Box>
                </VStack>
            </AbsoluteCenter>
        </>
    );
};

export default Login;
