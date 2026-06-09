import { Button } from "@/components/ui/button"
import { Card, CardAction, CardContent, CardDescription, CardFooter, CardHeader, CardTitle, } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { toast, Toaster } from "sonner";
import userApi from "@/services/userService";
import { LoginRequest, ProblemDetails } from "@/generated/dtos";

function LoginPage() {
	const location = useLocation()
	const navigate = useNavigate()

	const { initEmail = '', initPassword = '' } = location.state || {};

	const [email, setEmail] = useState<string>(initEmail)
	const [password, setPassword] = useState<string>(initPassword)

	const handleSubmit = async () => {
		try {
			await userApi.login(new LoginRequest({ email, password }))
			toast.success('Login successful!')
			navigate('/dashboard')
		} catch (e) {
			if (e instanceof ProblemDetails) {
				toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
			}
		}
	}

	return (
		<div className="flex h-screen items-center justify-center">
			<Card className="w-full max-w-sm">
				<CardHeader>
					<CardTitle>Login to your account</CardTitle>
					<CardDescription>
						Enter your email below to login to your account
					</CardDescription>
					<CardAction>
						<Button variant="link" onClick={() => navigate('/signup')}>Sign Up</Button>
					</CardAction>
				</CardHeader>
				<CardContent>
					<form>
						<div className="flex flex-col gap-6">
							<div className="grid gap-2">
								<Label htmlFor="email">Email</Label>
								<Input
									value={email}
									onChange={(e) => setEmail(e.target.value)}
									onKeyDown={(e) => e.key === 'Enter' && handleSubmit()}
									id="email"
									type="email"
									placeholder="m@example.com"
									required
								/>
							</div>
							<div className="grid gap-2">
								<div className="flex items-center">
									<Label htmlFor="password">Password</Label>
								</div>
								<Input
									id="password"
									type="password"
									required
									value={password}
									onChange={(e) => setPassword(e.target.value)}
									onKeyDown={(e) => e.key === 'Enter' && handleSubmit()}
								/>
							</div>
						</div>
					</form>
				</CardContent>
				<CardFooter className="flex-col gap-2">
					<Button type="submit" className="w-full" onClick={async () => handleSubmit()}>
						Login
					</Button>
				</CardFooter>
			</Card>
			<Toaster position="bottom-center" />
		</div>
	)
}

export default LoginPage