import {
    Box,
    Button,
    Field,
    Fieldset,
    Icon,
    Input,
    Stack,
} from "@chakra-ui/react";
import { useEffect, type FC } from "react";
import { Controller, useFormContext } from "react-hook-form";
import { BsArrowRight } from "react-icons/bs";
import type { LoginForm } from "../../../types/auth/types";

type FieldsetStateProps = {
    startFading: boolean;
    fadeDuration: number;
    isLoading: boolean;
    animationCompleted: () => void;
};

export const FieldsetState: FC<FieldsetStateProps> = ({
    startFading,
    fadeDuration,
    isLoading,
    animationCompleted,
}) => {
    const { control } = useFormContext<LoginForm>();

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
            <Fieldset.Root size="lg" minW={300}>
                <Stack>
                    <Fieldset.Legend>Вход</Fieldset.Legend>
                    <Fieldset.HelperText>
                        Введите телефон или почту
                    </Fieldset.HelperText>
                </Stack>

                <Fieldset.Content>
                    <Field.Root>
                        <Field.Label>Телефон или почта</Field.Label>
                        <Controller
                            name={"login"}
                            control={control}
                            render={({ field }) => (
                                <Input
                                    {...field}
                                    name="login"
                                    placeholder="Телефон или почта"
                                    value={field.value}
                                    onChange={(e) =>
                                        field.onChange(e.target.value)
                                    }
                                />
                            )}
                        />
                    </Field.Root>
                </Fieldset.Content>

                <Button
                    type="submit"
                    loading={isLoading}
                    loadingText="Отправка"
                >
                    Получить код
                    <Icon>
                        <BsArrowRight />
                    </Icon>
                </Button>
            </Fieldset.Root>
        </Box>
    );
};

export default FieldsetState;
