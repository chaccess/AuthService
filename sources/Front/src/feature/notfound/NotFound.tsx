import { AbsoluteCenter, Box, Button, Center, Text } from "@chakra-ui/react";
import type { FC } from "react";
import { useNavigate } from "react-router-dom";

export const NotFound: FC = () => {
    const redirect = useNavigate();

    return (
        <>
            <AbsoluteCenter>
                <Text opacity={"5%"} userSelect={"none"} fontSize={"400px"}>
                    {"<404/>"}
                </Text>
            </AbsoluteCenter>
            <AbsoluteCenter>
                <Box>
                    <Center mt={"100px"}>
                        <Text textAlign={"center"} fontSize={"6xl"}>
                            Кажется, ты оказался здесь по ошибке
                        </Text>
                    </Center>
                    <Center>
                        <Button mt={"50px"} onClick={() => redirect("/")}>
                            Вернуться к людям
                        </Button>
                    </Center>
                </Box>
            </AbsoluteCenter>
        </>
    );
};
