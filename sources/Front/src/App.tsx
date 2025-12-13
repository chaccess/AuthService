import type { FC } from "react";
import { LoginPage } from "./feature/auth/pages/LoginPage";
import { Provider } from "./components/ui/Provider";
import Header from "./components/header/Header";
import { Route, Routes } from "react-router-dom";
import { NotFound } from "./feature/notfound/NotFound";
import { RedirectToLogin } from "./feature/auth/components/RedirectToLogin";

export const App: FC = () => {
    return (
        <Provider>
            <Header></Header>
            <Routes>
                <Route path="/login" element={<LoginPage />} />
                <Route path="/" element={RedirectToLogin()} />
                <Route path="*" element={<NotFound />} />
            </Routes>
        </Provider>
    );
};

export default App;
