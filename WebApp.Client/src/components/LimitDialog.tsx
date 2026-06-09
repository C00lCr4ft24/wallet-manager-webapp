import { CreateLimitDto, FamilyUserDto, LimitDto, ProblemDetails } from "@/generated/dtos";
import { useEffect, useState } from "react";
import { Dialog, DialogClose, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle, DialogTrigger } from "./ui/dialog";
import { Button } from "./ui/button";
import { Field, FieldContent, FieldDescription, FieldGroup, FieldLabel } from "./ui/field";
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from "./ui/select";
import { Label } from "./ui/label";
import { Input } from "./ui/input";
import UserAvatarItem from "./UserAvatarItem";
import { type DateRange } from "react-day-picker"
import { addDays, format } from "date-fns";
import { Popover, PopoverContent, PopoverTrigger } from "./ui/popover";
import { Calendar } from "./ui/calendar";
import { CalendarIcon } from "lucide-react";
import limitService from "@/services/limitService";
import { toast } from "sonner";
import familyService from "@/services/familyService";
import { Checkbox } from "./ui/checkbox";


function LimitDialog({ children, onChanges, limit }: { children: React.ReactNode; onChanges?: () => void; limit?: LimitDto }) {
  const [open, setOpen] = useState(false)

  const [users, setUsers] = useState<FamilyUserDto[]>([])
  const [selectedUser, setSelectedUser] = useState<FamilyUserDto | undefined>(undefined)
  const [description, setDescription] = useState<string>('')
  const [amount, setAmount] = useState<string>('')
  const [includeIncome, setIncludeIncome] = useState(false)
  const [date, setDate] = useState<DateRange | undefined>({
    from: new Date(),
    to: addDays(new Date(), 31)
  })

  const editMode = !!limit

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const user = await familyService.GetCurrentFamilyUser()
        if (!user.isOwner) {
          const list: FamilyUserDto[] = []
          list.push(user)
          setUsers(list)
          setSelectedUser(user)
        }
        else {
          const allUsers = await familyService.GetFamilyUsers()
          setUsers(allUsers)
          setSelectedUser(allUsers[0])
        }

      } catch (e) {
        if (e instanceof ProblemDetails) {
          toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
        }
      }
    }
    fetchUsers()
  }, [])

  const handleSubmit = async () => {
    if (!selectedUser || !amount || !date?.from) {
      toast.error("Please fill in all required fields!", { position: 'bottom-center' })
      return
    }
    try {
      await limitService.createLimit(new CreateLimitDto({
        userId: selectedUser.id,
        maxAmount: parseFloat(amount),
        description: description,
        includeIncome: includeIncome,
        startDate: date.from,
        endDate: date.to,
      }))
      toast.success("Limit created successfully!", { position: 'bottom-center' })
      onChanges && onChanges()
      setOpen(false)
    } catch (e) {
      if (e instanceof ProblemDetails) {
        toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
      }
    }
  }

  return (
    <Dialog open={open} onOpenChange={(isOpen) => setOpen(isOpen)}>
      <form>
        <DialogTrigger asChild>
          {children}
        </DialogTrigger>
        <DialogContent className="sm:max-w-sm">
          <DialogHeader>
            <DialogTitle>{editMode ? 'Edit Limit' : 'Add Limit'}</DialogTitle>
            <DialogDescription>
              {editMode ? 'Edit the limit details and click save to apply changes.' : 'Fill in the details and click create to add a new limit.'}
            </DialogDescription>
          </DialogHeader>
          <FieldGroup>
            {!editMode && (
              <Field>
                <Label htmlFor="user">User</Label>
                <Select value={selectedUser?.id.toString() ?? '0'} onValueChange={(value) => setSelectedUser(users.find((u) => u.id.toString() === value))} disabled={users.length === 1}>
                  <SelectTrigger id="user">
                    <SelectValue placeholder="Select a user" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectGroup>
                      {users.map((user) => (
                        <SelectItem key={user.id} value={user.id?.toString() ?? '0'}>
                          <UserAvatarItem userId={user.user.id!} userName={user.user.userName!} />
                        </SelectItem>
                      ))}
                    </SelectGroup>
                  </SelectContent>
                </Select>
              </Field>
            )}
            <Field>
              <Label htmlFor="amount">Max Amount*</Label>
              <Input id="amount" name="amount" type="number" value={amount} onChange={(e) => setAmount(e.target.value)} />
            </Field>
            <Field>
              <Label htmlFor="description">Description</Label>
              <Input id="description" name="description" type="text" value={description} onChange={(e) => setDescription(e.target.value)} />
            </Field>
            <Field>
              <Label htmlFor="date">Date Range*</Label>
              <Popover>
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    id="date-picker-range"
                    className="justify-start px-2.5 font-normal"
                  >
                    <CalendarIcon />
                    {date?.from ? (
                      date.to ? (
                        <>
                          {format(date.from, "LLL dd, y")} -{" "}
                          {format(date.to, "LLL dd, y")}
                        </>
                      ) : (
                        format(date.from, "LLL dd, y")
                      )
                    ) : (
                      <span>Pick a date</span>
                    )}
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-auto p-0" align="start">
                  <Calendar
                    mode="range"
                    defaultMonth={date?.from}
                    selected={date}
                    onSelect={setDate}
                    numberOfMonths={2}
                  />
                </PopoverContent>
              </Popover>
            </Field>
          </FieldGroup>
          <FieldGroup className="max-w-sm">
            <Field orientation="horizontal">
              <Checkbox
                id="include-income-checkbox"
                name="include-income-checkbox"
                checked={includeIncome}
                onCheckedChange={(checked) => setIncludeIncome(checked === true)}
              />
              <FieldContent>
                <FieldLabel htmlFor="include-income-checkbox">
                  Include Income
                </FieldLabel>
                <FieldDescription>
                  Include income in the limit calculation.
                </FieldDescription>
              </FieldContent>
            </Field>
          </FieldGroup>
          <DialogFooter>
            <DialogClose asChild>
              <Button variant="outline">Cancel</Button>
            </DialogClose>
            <Button type="submit" onClick={() => { handleSubmit() }}>
              Create
            </Button>
          </DialogFooter>
        </DialogContent>
      </form>
    </Dialog>
  )
}

export default LimitDialog