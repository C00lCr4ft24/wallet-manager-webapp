import { LoginRequest, RegisterUserDto, UpdateUserDateOfBirthDto, UpdateUserEmailDto, UpdateUserNameDto, UpdateUserPasswordDto } from "@/generated/dtos";
import createFetch from "@/lib/fetchwrapper";
import { clearBearerToken, setBearerToken } from "@/lib/tokenaccessor";

async function login(request: LoginRequest) {
	const data = await createFetch('/api/user/login', 'POST', request)
	setBearerToken(data.accessToken)
	return data
}

async function logout() {
    const data = await createFetch('/api/user/logout', 'POST')
    clearBearerToken()
	return data
}

async function register(request: RegisterUserDto) {
	const data = await createFetch('/api/user/register-user', 'POST', request)
	return data
}

async function currentUser() {
	const data = await createFetch('/api/user/current-user', 'GET')
	return data
}

async function updateEmail(request: UpdateUserEmailDto) {
	const data = await createFetch('/api/user/update-email', 'POST', request)
	return data
}

async function updateUserName(request: UpdateUserNameDto) {
	const data = await createFetch('/api/user/update-username', 'POST', request)
	return data
}

async function updateDateOfBirth(request: UpdateUserDateOfBirthDto) {
	const data = await createFetch('/api/user/update-dob', 'POST', request)
	return data
}

async function updatePassword(request: UpdateUserPasswordDto) {
	const data = await createFetch('/api/user/update-password', 'POST', request)
	return data
}

const userService = {
	login,
	logout,
	register,
	currentUser,
	updateEmail,
	updateUserName,
	updateDateOfBirth,
	updatePassword
}

export default userService