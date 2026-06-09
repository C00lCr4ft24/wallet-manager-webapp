import { createBrowserRouter, RouterProvider, redirect } from "react-router-dom"
import NotFoundPage from "./pages/NotFoundPage";
import DashboardPage from "./pages/DashboardPage";
import { Spinner } from "./components/ui/spinner";
import MainLayout from "./layouts/MainLayout";
import RootPage from "./pages/RootPage";
import LoginPage from "./pages/LoginPage";
import SignupPage from "./pages/SignUpPage";
import userApi from "./services/userService";
import FamilyPage from "./pages/FamilyPage";
import WalletsPage from "./pages/WalletsPage";
import TransactionsPage from "./pages/TransactionsPage";
import CategoriesPage from "./pages/CategoriesPage";
import LimitsPage from "./pages/LimitsPage";
import ProfilePage from "./pages/ProfilePage";
import FamilyUserItemPage from "./pages/FamilyUserItemPage";
import WalletItemPage from "./pages/WalletItemPage";
import WalletUserItemPage from "./pages/WalletUserItemPage";

export const authLoader = async () => {
	try {
		const response = await userApi.currentUser()
		return response
	} catch (error) {
		throw redirect('/login')
	}
}

const SpinnerFallback = () => (
	<div className="flex h-screen items-center justify-center">
		<Spinner className="size-8" />
	</div>
)

const router = createBrowserRouter([
	{
		path: '/',
		element: <RootPage />,
		errorElement: <NotFoundPage />
	},
	{
		hydrateFallbackElement: <SpinnerFallback />,
		children: [
			{ path: 'login', element: <LoginPage /> },
			{ path: 'signup', element: <SignupPage /> }
		]
	},
	{
		element: <MainLayout />,
		loader: authLoader,
		hydrateFallbackElement: <SpinnerFallback />,
		children: [
			{ path: 'dashboard', element: <DashboardPage /> },
			{ path: 'family', element: <FamilyPage /> },
			{ path: 'wallets', element: <WalletsPage /> },
			{ path: 'transactions', element: <TransactionsPage /> },
			{ path: 'categories', element: <CategoriesPage /> },
			{ path: 'limits', element: <LimitsPage /> },
			{ path: 'profile', element: <ProfilePage /> },
			{ path: 'family/user/:familyUserId', element: <FamilyUserItemPage /> },
			{ path: 'wallets/:walletId', element: <WalletItemPage /> },
			{ path: 'wallets/user/:walletUserId', element: <WalletUserItemPage /> },

		]
	}
])

export default function App() {
	return (<RouterProvider router={router} />)
}