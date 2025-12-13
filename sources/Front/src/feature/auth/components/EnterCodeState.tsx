import {
    Box,
    Button,
    Field,
    Fieldset,
    Icon,
    Link,
    Loader,
    PinInput,
    Stack,
    VStack,
    Text,
} from "@chakra-ui/react";
import {
    useEffect,
    useRef,
    useState,
    type Dispatch,
    type FC,
    type SetStateAction,
} from "react";
import { BsArrowLeft } from "react-icons/bs";
import type { LoginType } from "../../../api/auth/types";
import { Controller, useFormContext } from "react-hook-form";
import { type LoginForm } from "../../../types/auth/types";
import { verifiCodeAsync } from "../../../api/auth";

type EnterCodeStateProps = {
    startFading: boolean;
    fadeDuration: number;
    loginType: LoginType;
    animationCompleted: () => void;
    backToFieldsForm: () => void;
    resendCode: () => void;
    secondsLeft: number;
    isRunning: boolean;
    setVerified: Dispatch<SetStateAction<boolean>>;
    setGetTokenCode: Dispatch<SetStateAction<string>>;
};

export const EnterCodeState: FC<EnterCodeStateProps> = ({
    startFading,
    fadeDuration,
    loginType,
    animationCompleted,
    backToFieldsForm,
    resendCode,
    secondsLeft,
    isRunning,
    setVerified,
    setGetTokenCode,
}) => {
    const { setValue, watch, handleSubmit, control } =
        useFormContext<LoginForm>();
    const firstPinRef = useRef<HTMLInputElement>(null);
    const lastPinRef = useRef<HTMLInputElement>(null);
    const [isDisabled, setIsDisabled] = useState<boolean>(false);
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [verifyError, setVerifyError] = useState<boolean>(false);
    const code = watch("code");
    const makeEmptyCode = () => Array(6).fill("");

    const verifyCode = async (data: LoginForm) => {
        setIsLoading(true);
        try {
            var res = await verifiCodeAsync(data);

            if (res.isSuccess) {
                setGetTokenCode(res.code);
                setVerified(true);
            }
        } catch {
            setVerifyError(true);
        } finally {
            setIsLoading(false);
            setValue("code", makeEmptyCode());
            setIsDisabled(false);
            firstPinRef.current?.focus();
            lastPinRef.current?.setAttribute("placeholder", "○"); // <-- Костыль, не знаю как иначе победить
        }
    };

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
        if (code?.length == 6 && code.every((x) => x.length == 1)) {
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
                                {loginType === "email" ? " почту " : " номер "}
                                {field.value}
                            </Fieldset.HelperText>
                        </VStack>

                        <Fieldset.Content>
                            <Field.Root>
                                <Field.Label>Код</Field.Label>
                                <PinInput.Root
                                    minW={300}
                                    autoFocus
                                    disabled={isDisabled}
                                    invalid={verifyError}
                                    colorPalette="chocolate"
                                    otp
                                    value={field.value}
                                    onValueChange={(e) => {
                                        field.onChange(e.value);
                                        setVerifyError(false);
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
                                            ref={lastPinRef}
                                            caretColor={"transparent"}
                                        />
                                    </PinInput.Control>
                                </PinInput.Root>
                            </Field.Root>
                        </Fieldset.Content>
                        <Stack>
                            <Button
                                alignSelf="flex-start"
                                onClick={() => {
                                    setValue("code", makeEmptyCode());
                                    backToFieldsForm();
                                }}
                            >
                                <Icon>
                                    <BsArrowLeft />
                                </Icon>
                            </Button>
                            {isRunning && secondsLeft !== 60 && (
                                <Text>Осталось {secondsLeft} секунд</Text>
                            )}
                            {!isRunning && (
                                <Link
                                    variant="underline"
                                    onClick={() => {
                                        resendCode();
                                        setVerifyError(false);
                                    }}
                                >
                                    Отправить заново
                                </Link>
                            )}
                        </Stack>
                    </Fieldset.Root>
                )}
            />
        </Box>
    );
};
