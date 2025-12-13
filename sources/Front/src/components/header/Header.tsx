import { Box, Flex, Heading, Image } from "@chakra-ui/react";
import type { FC } from "react";
import { ColorModeButton } from "../ui/ColorMode";
import { useNavigate } from "react-router-dom";

type HeaderProps = {};

export const Header: FC<HeaderProps> = () => {
    const redirect = useNavigate();
    return (
        <Box borderBottomWidth={"5px"} rounded={"4xl"}>
            <Flex justify="space-between" align="center" pr={"50px"}>
                <Flex align="center">
                    <Box padding={30}>
                        <Flex
                            align="center"
                            onClick={() => redirect("/")}
                            cursor={"pointer"}
                        >
                            <Image src="/icon.png" boxSize="100px" />
                            <Heading size="5xl">shokolade</Heading>
                        </Flex>
                    </Box>
                </Flex>

                <ColorModeButton />
            </Flex>
        </Box>
    );
};

export default Header;
