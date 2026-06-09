import { Button } from "@/components/ui/button"
import { Card, CardAction, CardContent, CardDescription, CardFooter, CardHeader, CardTitle, } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { toast, Toaster } from "sonner"
import userApi from "@/services/userService";
import { RegisterRequest, ProblemDetails } from "@/generated/dtos";

function SignUpPage() {

	const [email, setEmail] = useState<string>('')
	const [password, setPassword] = useState<string>('')
	const [passwordAgain, setPasswordAgain] = useState<string>('')
	const navigate = useNavigate()

	const handleSubmit = async () => {
		try {
			await userApi.register(new RegisterRequest({ email, password }))
			navigate('/login', { state: { initEmail: email, initPassword: password, onCall: true } })
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
					<CardTitle>Register an account</CardTitle>
					<CardDescription>
						Enter your email below to register an account
					</CardDescription>
					<CardAction>
						<Button variant="link" onClick={() => navigate('/login')}>Login</Button>
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
									value={password}
									onChange={(e) => setPassword(e.target.value)}
									onKeyDown={(e) => e.key === 'Enter' && handleSubmit()}
									id="password"
									type="password"
									required
								/>
							</div>
							<div className="grid gap-2">
								<div className="flex items-center">
									<Label htmlFor="password">Password again</Label>
								</div>
								<Input
									value={passwordAgain}
									onChange={(e) => setPasswordAgain(e.target.value)}
									onKeyDown={(e) => e.key === 'Enter' && handleSubmit()}
									id="password"
									type="password"
									required
								/>
							</div>
						</div>
					</form>
				</CardContent>
				<CardFooter className="flex-col gap-2">
					<Button type="submit" className="w-full" onClick={() => handleSubmit()}>
						Register
					</Button>
				</CardFooter>
			</Card>
			<Toaster position="bottom-center" />
		</div>
	)
}

export default SignUpPage