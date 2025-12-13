import { Navigate, useLocation } from "react-router-dom";

export function RedirectToLogin() {
    const location = useLocation();
    return <Navigate to={`/login${location.search}`} replace />;
}
