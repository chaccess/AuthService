import { AbsoluteCenter, Box, VStack } from "@chakra-ui/react";
import { useEffect, useState, type FC } from "react";
import { FormProvider, useForm } from "react-hook-form";
import FieldsetState from "./FieldsetState";
import { EnterCodeState } from "./EnterCodeState";
import type { LoginFormState } from "./types/AuthTypes";
import { sendCodeAsync, verifiCodeAsync } from "../../../api/auth";
import type { ISendCodeResult } from "../../../api/auth/types";
import type { LoginForm } from "../../../types/auth/types";
import { LoginFormDefaults } from "../../../types/auth/defaults";

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
    const [isLoadingCode, setIsLoadingCode] = useState<boolean>(false);
    const fadeDuration = 300;

    const { handleSubmit } = methods;

    const sendCode = async (data: LoginForm) => {
        setIsLoadingFields(true);
        try {
            const res = await sendCodeAsync(data.login);
            console.log(res);
            setSendCodeResult(res);
        } catch {
        } finally {
            setIsLoadingFields(false);
        }
    };

    const verifyCode = async (data: LoginForm) => {
        setIsLoadingCode(true);
        try {
            const res = await verifiCodeAsync(data);
            console.log({ res });
        } catch {
        } finally {
            setIsLoadingCode(false);
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

    console.log(sendCodeResult?.loginType);

    return (
        <>
            <AbsoluteCenter>
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
                                        verifyCode={verifyCode}
                                        startFading={!fadingFields}
                                        fadeDuration={fadeDuration}
                                        loginType={sendCodeResult.loginType}
                                        animationCompleted={animationCompleted}
                                        backToFieldsForm={backToFieldsForm}
                                        isLoading={isLoadingCode}
                                    />
                                )
                            )}
                        </FormProvider>
                    </VStack>
                </Box>
            </AbsoluteCenter>
        </>
    );
};

export default Login;
