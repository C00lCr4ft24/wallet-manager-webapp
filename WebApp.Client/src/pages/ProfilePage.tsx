import { Button } from "@/components/ui/button"
import { Calendar } from "@/components/ui/calendar"
import { Card, CardAction, CardContent, CardDescription, CardFooter, CardHeader, CardTitle, } from "@/components/ui/card"
import { Field, FieldLabel } from "@/components/ui/field"
import { Input } from "@/components/ui/input"
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover"
import { ProblemDetails, UpdateUserDateOfBirthDto, UpdateUserEmailDto, UpdateUserNameDto, UpdateUserPasswordDto, UserHeaderDto } from "@/generated/dtos"
import userService from "@/services/userService"
import { useEffect, useState } from "react"
import { toast } from "sonner"
import { parseISO } from "date-fns"

function ProfilePage() {
  const [email, setEmail] = useState<string>('')
  const [enableEmailEdits, setEnableEmailEdits] = useState<boolean>(false)

  const [userName, setUserName] = useState<string>('')
  const [enableUserNameEdits, setEnableUserNameEdits] = useState<boolean>(false)

  const [password, setPassword] = useState<string>('')
  const [confirmPassword, setConfirmPassword] = useState<string>('')
  const [currentPassword, setCurrentPassword] = useState<string>('')
  const [enablePasswordEdits, setEnablePasswordEdits] = useState<boolean>(false)

  const [dateOfBirth, setDateOfBirth] = useState<Date | undefined>(undefined)
  const [enableDateOfBirthEdits, setEnableDateOfBirthEdits] = useState<boolean>(false)
  const [open, setOpen] = useState(false)


  const [user, setUser] = useState<UserHeaderDto | undefined>(undefined)

  useEffect(() => {
    const fetchUser = async () => {
      try {
        const user = await userService.currentUser()
        setUser(user)
        setEmail(user.email)
        setUserName(user.userName)
        if (user.dateOfBirth && user.dateOfBirth !== '0001-01-01T00:00:00') {
          const parsedDate = parseISO(user.dateOfBirth)
          setDateOfBirth(parsedDate)
        }
      } catch (e) {
        if (e instanceof ProblemDetails) {
          toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
        }
      }
    }
    fetchUser()
    console.log(dateOfBirth)
  }, [])

  const handleSubmit = async () => {
    const normalizedDate = dateOfBirth
      ? new Date(Date.UTC(dateOfBirth.getFullYear(), dateOfBirth.getMonth(), dateOfBirth.getDate()))
      : undefined
    const emailChanged = email !== user?.email
    const userNameChanged = userName !== user?.userName
    const dateOfBirthChanged = normalizedDate?.toISOString() !== user?.dateOfBirth
    const passwordChanged = password !== '' || confirmPassword !== '' || currentPassword !== ''
    try {
      if (emailChanged) {
        await userService.updateEmail(new UpdateUserEmailDto({ email }))
        setEnableEmailEdits(false)
        setEmail(email)
      }
      if (userNameChanged) {
        await userService.updateUserName(new UpdateUserNameDto({ userName: userName }))
        setEnableUserNameEdits(false)
        setUserName(userName)
      }
      if (dateOfBirthChanged) {
        await userService.updateDateOfBirth(new UpdateUserDateOfBirthDto({ dateOfBirth: normalizedDate }))
        setEnableDateOfBirthEdits(false)
        setDateOfBirth(dateOfBirth)
      }
      if (passwordChanged) {
        if (password !== confirmPassword) {
          toast.error("New password and confirm password do not match!", { position: 'bottom-center' })
          return
        }
        await userService.updatePassword(new UpdateUserPasswordDto({ oldPassword: currentPassword, newPassword: password }))
        setEnablePasswordEdits(false)
        setPassword('')
        setConfirmPassword('')
        setCurrentPassword('')
      }
      toast.success("Profile updated successfully!", { position: 'bottom-center' })
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
          <CardTitle>Your Profile</CardTitle>
          <CardDescription>
            Manage your profile information
          </CardDescription>
          <CardAction>
            <Button variant="link" onClick={() => window.history.back()}>
              Back
            </Button>
          </CardAction>
        </CardHeader>
        <CardContent>
          <Field className="flex flex-col" orientation="vertical">
            <FieldLabel htmlFor="email"> Email </FieldLabel>
            <Field orientation="horizontal">
              <Input
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                onKeyDown={(e) => e.key === 'Enter' && handleSubmit()}
                id="email"
                type="email"
                placeholder="loading..."
                required
                disabled={!enableEmailEdits}
              />
              <Button
                variant={enableEmailEdits ? "default" : "outline"}
                className="ml-auto mt-2"
                onClick={() => setEnableEmailEdits(!enableEmailEdits)}
                onKeyDown={(e) => { if (e.key === 'Enter') { setEnableEmailEdits(!enableEmailEdits) } }}
              >
                {enableEmailEdits ? 'Cancel' : 'Edit'}
              </Button>
            </Field>
            <FieldLabel htmlFor="username"> Username </FieldLabel>
            <Field orientation="horizontal">
              <Input
                value={userName}
                onChange={(e) => setUserName(e.target.value)}
                onKeyDown={(e) => e.key === 'Enter' && handleSubmit()}
                id="username"
                type="text"
                placeholder="loading..."
                required
                disabled={!enableUserNameEdits}
              />
              <Button
                variant={enableUserNameEdits ? "default" : "outline"}
                className="ml-auto mt-2"
                onClick={() => setEnableUserNameEdits(!enableUserNameEdits)}
                onKeyDown={(e) => { if (e.key === 'Enter') { setEnableUserNameEdits(!enableUserNameEdits) } }}
              >
                {enableUserNameEdits ? 'Cancel' : 'Edit'}
              </Button>
            </Field>
            <FieldLabel htmlFor="date">Date of birth</FieldLabel>
            <Field orientation="horizontal">
              <Popover open={open} onOpenChange={setOpen}>
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    id="date"
                    className="justify-start font-normal"
                    disabled={!enableDateOfBirthEdits}
                    onKeyDown={(e) => { if (e.key === 'Enter') { setOpen(true) } }}
                  >
                    {dateOfBirth ? dateOfBirth.toLocaleDateString() : "Set Date of Birth"}
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-auto overflow-hidden p-0" align="start">
                  <Calendar
                    mode="single"
                    selected={dateOfBirth}
                    defaultMonth={dateOfBirth}
                    captionLayout="dropdown"
                    onSelect={(date) => {
                      setDateOfBirth(date)
                      setOpen(false)
                    }}
                  />
                </PopoverContent>
              </Popover>
              <Button
                variant={enableDateOfBirthEdits ? "default" : "outline"}
                className="ml-auto"
                onClick={() => setEnableDateOfBirthEdits(!enableDateOfBirthEdits)}
                onKeyDown={(e) => { if (e.key === 'Enter') { setEnableDateOfBirthEdits(!enableDateOfBirthEdits) } }}
              >
                {enableDateOfBirthEdits ? 'Cancel' : 'Edit'}
              </Button>
            </Field>
            <FieldLabel htmlFor="new-password"> {enablePasswordEdits ? 'New Password' : 'Password'} </FieldLabel>
            <Field orientation="horizontal">
              <Input
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                onKeyDown={(e) => e.key === 'Enter' && handleSubmit()}
                id="new-password"
                type="password"
                placeholder="********"
                required
                disabled={!enablePasswordEdits}
              />
              <Button
                variant={enablePasswordEdits ? "default" : "outline"}
                className="ml-auto mt-2"
                onClick={() => setEnablePasswordEdits(!enablePasswordEdits)}
                onKeyDown={(e) => { if (e.key === 'Enter') { setEnablePasswordEdits(!enablePasswordEdits) } }}
              >
                {enablePasswordEdits ? 'Cancel' : 'Edit'}
              </Button>
            </Field>
            {enablePasswordEdits && (

              <Field orientation="vertical">
                <FieldLabel htmlFor="new-password-again"> New Password Again </FieldLabel>
                <Field orientation="horizontal">

                  <Input
                    value={confirmPassword}
                    onChange={(e) => setConfirmPassword(e.target.value)}
                    onKeyDown={(e) => e.key === 'Enter' && handleSubmit()}
                    id="new-password-again"
                    type="password"
                    placeholder="********"
                    required
                    disabled={!enablePasswordEdits}
                  />
                </Field>

                <FieldLabel htmlFor="current-password"> Current Password </FieldLabel>
                <Field orientation="horizontal">

                  <Input
                    value={currentPassword}
                    onChange={(e) => setCurrentPassword(e.target.value)}
                    onKeyDown={(e) => e.key === 'Enter' && handleSubmit()}
                    id="current-password"
                    type="password"
                    placeholder="********"
                    required
                    disabled={!enablePasswordEdits}
                  />
                </Field>
              </Field>
            )}
          </Field>
        </CardContent>
        <CardFooter className="flex-col gap-2">
          <Button type="submit" className="w-full" onClick={async () => handleSubmit()}>
            Update
          </Button>
        </CardFooter>
      </Card>
    </div >
  )
}

export default ProfilePage