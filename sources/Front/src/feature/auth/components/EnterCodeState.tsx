import {
    Box,
    Button,
    Field,
    Fieldset,
    Icon,
    Loader,
    PinInput,
    VStack,
} from "@chakra-ui/react";
import { useEffect, useRef, useState, type FC } from "react";
import { BsArrowLeft } from "react-icons/bs";
import type { LoginType } from "../../../api/auth/types";
import { Controller, useFormContext } from "react-hook-form";
import { type LoginForm } from "../../../types/auth/types";

type EnterCodeStateProps = {
    startFading: boolean;
    fadeDuration: number;
    loginType: LoginType;
    isLoading: boolean;
    animationCompleted: () => void;
    backToFieldsForm: () => void;
    verifyCode: (data: LoginForm) => Promise<void>;
};

export const EnterCodeState: FC<EnterCodeStateProps> = ({
    startFading,
    fadeDuration,
    loginType,
    isLoading,
    animationCompleted,
    backToFieldsForm,
    verifyCode,
}) => {
    const { setValue, watch, handleSubmit, control } =
        useFormContext<LoginForm>();
    const firstPinRef = useRef<HTMLInputElement>(null);
    const [isDisabled, setIsDisabled] = useState<boolean>(false);
    const login = watch("login");
    const code = watch("code");

    useEffect(() => {
        if (startFading) {
            const timeout = setTimeout(() => {
                animationCompleted();
            }, fadeDuration - 50);
            return () => {
                clearTimeout(timeout);
            };
        }
    }, [startFading]);

    useEffect(() => {
        const handleKeyDown = (e: KeyboardEvent) => {
            if (firstPinRef.current?.value) return;
            const allowedKeys =
                e.key.length == 1 ||
                (e.key === "Backspace" &&
                    !e.ctrlKey &&
                    !e.altKey &&
                    !e.metaKey);

            if (!allowedKeys) return;

            firstPinRef.current?.focus();
        };

        window.addEventListener("keydown", handleKeyDown);

        return () => window.removeEventListener("keydown", handleKeyDown);
    }, []);

    useEffect(() => {
        if (code?.length == 6) {
            setIsDisabled(true);
            handleSubmit(verifyCode)();
        }
    }, [code]);

    return (
        <Box
            data-state={!startFading ? "open" : "closed"}
            _open={{
                animation: `fade-in ${fadeDuration}ms`,
            }}
            _closed={{
                animation: `fade-out ${fadeDuration}ms`,
            }}
        >
            <Loader visible={isLoading}></Loader>
            <Controller
                name="code"
                control={control}
                render={({ field }) => (
                    <Fieldset.Root size="lg" minW={300}>
                        <VStack>
                            <Fieldset.Legend>Вход</Fieldset.Legend>
                            <Fieldset.HelperText>
                                Код отправлен на
                                {loginType === "email" ? " Email " : " номер "}
                                {login}
                            </Fieldset.HelperText>
                        </VStack>

                        <Fieldset.Content>
                            <Field.Root>
                                <Field.Label>Код</Field.Label>
                                <PinInput.Root
                                    minW={300}
                                    autoFocus
                                    disabled={isDisabled}
                                    otp
                                    onValueChange={(e) => {
                                        field.onChange(e.value.join(""));
                                    }}
                                >
                                    <PinInput.HiddenInput />
                                    <PinInput.Control>
                                        <PinInput.Input
                                            index={0}
                                            ref={firstPinRef}
                                            caretColor={"transparent"}
                                        />
                                        <PinInput.Input
                                            index={1}
                                            caretColor={"transparent"}
                                        />
                                        <PinInput.Input
                                            index={2}
                                            caretColor={"transparent"}
                                        />
                                        <PinInput.Input
                                            index={3}
                                            caretColor={"transparent"}
                                        />
                                        <PinInput.Input
                                            index={4}
                                            caretColor={"transparent"}
                                        />
                                        <PinInput.Input
                                            index={5}
                                            caretColor={"transparent"}
                                        />
                                    </PinInput.Control>
                                </PinInput.Root>
                            </Field.Root>
                        </Fieldset.Content>

                        <Button
                            alignSelf="flex-start"
                            onClick={() => {
                                setValue("code", "");
                                backToFieldsForm();
                            }}
                        >
                            <Icon>
                                <BsArrowLeft />
                            </Icon>
                        </Button>
                    </Fieldset.Root>
                )}
            />
        </Box>
    );
};
