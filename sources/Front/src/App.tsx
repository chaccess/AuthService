import type { FC } from "react";
import { LoginPage } from "./feature/auth/pages/LoginPage";
import { Provider } from "./components/ui/provider";
import { ColorModeButton } from "./components/ui/color-mode";

export const App: FC = () => {
    return (
        <Provider>
            <ColorModeButton></ColorModeButton>
            <LoginPage></LoginPage>
        </Provider>
    );
};

export default App;
