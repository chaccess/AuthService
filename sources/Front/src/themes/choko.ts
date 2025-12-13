import { createSystem, defaultConfig, defineConfig } from "@chakra-ui/react";

const chocolate = {
    50: { value: "#FFF7E8" },
    100: { value: "#F5DAB0" },
    200: { value: "#E7B77A" },
    300: { value: "#D89A54" },
    400: { value: "#C67A30" },
    500: { value: "#A55A1B" },
    600: { value: "#7C3E14" },
    700: { value: "#582A10" },
    800: { value: "#3A1A0C" },
    900: { value: "#140904" },
};

export const system = createSystem(
    defaultConfig,
    defineConfig({
        theme: {
            tokens: {
                colors: { chocolate },
            },

            semanticTokens: {
                colors: {
                    //
                    // === BACKGROUND ===
                    //
                    bg: {
                        DEFAULT: {
                            value: {
                                _light: "{colors.chocolate.50}",
                                _dark: "{colors.chocolate.900}",
                            },
                        },
                        subtle: {
                            value: {
                                _light: "{colors.chocolate.100}",
                                _dark: "{colors.chocolate.800}",
                            },
                        },
                        muted: {
                            value: {
                                _light: "{colors.chocolate.200}",
                                _dark: "{colors.chocolate.700}",
                            },
                        },
                        emphasized: {
                            value: {
                                _light: "{colors.chocolate.300}",
                                _dark: "{colors.chocolate.600}",
                            },
                        },
                        inverted: {
                            value: {
                                _light: "{colors.chocolate.900}",
                                _dark: "{colors.chocolate.50}",
                            },
                        },
                        panel: {
                            value: {
                                _light: "{colors.chocolate.100}",
                                _dark: "{colors.chocolate.800}",
                            },
                        },

                        // статусные — читаемые, но тёплые
                        error: {
                            value: {
                                _light: "{colors.red.50}",
                                _dark: "{colors.red.900}",
                            },
                        },
                        warning: {
                            value: {
                                _light: "{colors.orange.50}",
                                _dark: "{colors.orange.900}",
                            },
                        },
                        success: {
                            value: {
                                _light: "{colors.green.50}",
                                _dark: "{colors.green.900}",
                            },
                        },
                        info: {
                            value: {
                                _light: "{colors.blue.50}",
                                _dark: "{colors.blue.900}",
                            },
                        },
                    },

                    //
                    // === FOREGROUND / TEXT ===
                    //
                    fg: {
                        DEFAULT: {
                            value: {
                                _light: "{colors.chocolate.900}",
                                _dark: "{colors.chocolate.50}",
                            },
                        },
                        muted: {
                            value: {
                                _light: "{colors.chocolate.700}",
                                _dark: "{colors.chocolate.200}",
                            },
                        },
                        subtle: {
                            value: {
                                _light: "{colors.chocolate.500}",
                                _dark: "{colors.chocolate.300}",
                            },
                        },
                        inverted: {
                            value: {
                                _light: "{colors.chocolate.50}",
                                _dark: "{colors.chocolate.900}",
                            },
                        },

                        error: {
                            value: {
                                _light: "{colors.red.600}",
                                _dark: "{colors.red.300}",
                            },
                        },
                        warning: {
                            value: {
                                _light: "{colors.orange.600}",
                                _dark: "{colors.orange.200}",
                            },
                        },
                        success: {
                            value: {
                                _light: "{colors.green.600}",
                                _dark: "{colors.green.200}",
                            },
                        },
                        info: {
                            value: {
                                _light: "{colors.blue.600}",
                                _dark: "{colors.blue.200}",
                            },
                        },
                    },

                    //
                    // === BORDER ===
                    //
                    border: {
                        DEFAULT: {
                            value: {
                                _light: "{colors.chocolate.700}",
                                _dark: "{colors.chocolate.200}",
                            },
                        },
                        subtle: {
                            value: {
                                _light: "{colors.chocolate.200}",
                                _dark: "{colors.chocolate.700}",
                            },
                        },
                        muted: {
                            value: {
                                _light: "{colors.chocolate.100}",
                                _dark: "{colors.chocolate.800}",
                            },
                        },
                        emphasized: {
                            value: {
                                _light: "{colors.chocolate.500}",
                                _dark: "{colors.chocolate.600}",
                            },
                        },
                        inverted: {
                            value: {
                                _light: "{colors.chocolate.50}",
                                _dark: "{colors.chocolate.900}",
                            },
                        },

                        error: {
                            value: {
                                _light: "{colors.red.400}",
                                _dark: "{colors.red.500}",
                            },
                        },
                        warning: {
                            value: {
                                _light: "{colors.orange.400}",
                                _dark: "{colors.orange.500}",
                            },
                        },
                        success: {
                            value: {
                                _light: "{colors.green.400}",
                                _dark: "{colors.green.500}",
                            },
                        },
                        info: {
                            value: {
                                _light: "{colors.blue.400}",
                                _dark: "{colors.blue.500}",
                            },
                        },
                    },

                    //
                    // === GRAY (переопределяем нейтралку → шоколад) ===
                    //
                    gray: {
                        contrast: {
                            value: {
                                _light: "{colors.chocolate.900}",
                                _dark: "{colors.chocolate.50}",
                            },
                        },
                        fg: {
                            value: {
                                _light: "{colors.chocolate.800}",
                                _dark: "{colors.chocolate.200}",
                            },
                        },
                        subtle: {
                            value: {
                                _light: "{colors.chocolate.200}",
                                _dark: "{colors.chocolate.800}",
                            },
                        },
                        muted: {
                            value: {
                                _light: "{colors.chocolate.200}",
                                _dark: "{colors.chocolate.800}",
                            },
                        },
                        emphasized: {
                            value: {
                                _light: "{colors.chocolate.400}",
                                _dark: "{colors.chocolate.600}",
                            },
                        },
                        solid: {
                            value: {
                                _light: "{colors.chocolate.600}",
                                _dark: "{colors.chocolate.400}",
                            },
                        },
                        focusRing: {
                            value: {
                                _light: "{colors.chocolate.400}",
                                _dark: "{colors.chocolate.500}",
                            },
                        },
                        border: {
                            value: {
                                _light: "{colors.chocolate.300}",
                                _dark: "{colors.chocolate.700}",
                            },
                        },
                    },

                    //
                    // === chocolate palette semantic ===
                    //
                    chocolate: {
                        solid: {
                            value: {
                                _light: "{colors.chocolate.700}",
                                _dark: "{colors.chocolate.200}",
                            },
                        },
                        contrast: {
                            value: {
                                _light: "{colors.chocolate.50}",
                                _dark: "{colors.chocolate.900}",
                            },
                        },
                        fg: {
                            value: {
                                _light: "{colors.chocolate.900}",
                                _dark: "{colors.chocolate.50}",
                            },
                        },
                        muted: {
                            value: {
                                _light: "{colors.chocolate.200}",
                                _dark: "{colors.chocolate.800}",
                            },
                        },
                        subtle: {
                            value: {
                                _light: "{colors.chocolate.100}",
                                _dark: "{colors.chocolate.800}",
                            },
                        },
                        emphasized: {
                            value: {
                                _light: "{colors.chocolate.600}",
                                _dark: "{colors.chocolate.300}",
                            },
                        },
                        focusRing: {
                            value: {
                                _light: "{colors.chocolate.400}",
                                _dark: "{colors.chocolate.300}",
                            },
                        },
                    },
                },
            },
        },

        //
        // === GLOBAL CSS FOR BODY ===
        //
        globalCss: {
            html: {
                colorPalette: "chocolate",
            },
            body: {
                bg: "bg",
                color: "fg",
                transitionProperty: "background-color, color",
                transitionDuration: "1000ms",
                transitionTimingFunction: "ease",
            },

            // плавно анимируем основные визуальные свойства у большинства элементов
            ".chakra-theme, .chakra-theme *": {
                transitionProperty:
                    "background-color, color, border-color, fill, stroke, box-shadow",
                transitionDuration: "1000ms",
                transitionTimingFunction: "ease",
            },
        },
    })
);
