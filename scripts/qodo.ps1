param(
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$Args
)

docker compose exec node-tools qodo @Args